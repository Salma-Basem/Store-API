using AutoMapper;
using StackExchange.Redis;
using Store.Data.Entities;
using Store.Data.Entities.OrderEntities;
using Store.Repository.Interfaces;
using Store.Repository.Specification.OrderSpecs;
using Store.Service.Services.BasketService;
using Store.Service.Services.OrderService.Dtos;
using Store.Service.Services.PaymentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = Store.Data.Entities.OrderEntities.Order;

namespace Store.Service.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBasketService _basketService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketService basketService
            ,IUnitOfWork unitOfWork,IMapper mapper,IPaymentService paymentService)
        {
           _basketService = basketService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
           _paymentService = paymentService;
        }
        public async Task<OrderDetailsDto> CreateOrderAsync(OrderDto input)
        {
            var basket = await _basketService.GetBasketAsync(input.BasketId);
            if (basket is null)
                throw new Exception("Basket Not Exist");
            var orderItems = new List<OrderItemDto>();
            foreach(var basketItem in basket.BasketItems)
            {
                var productItem = await _unitOfWork.Repository<Product,int>().GetByIdAsync(basketItem.ProductId);

                if(productItem is null)
                    throw new Exception($"Product With Id: {basketItem.ProductId} Not Exist");
                var itemOrdered = new ProductItem
                {
                    ProductId = productItem.Id,
                    ProductName = productItem.Name,
                    PictureUrl = productItem.PictureUrl
                };

                var orderItem = new OrderItem
                {
                    Price = productItem.Price,
                    Quantity=basketItem.Quantity,
                    ProductItem=itemOrdered
                    
                };

                var mappedOrderItem = _mapper.Map<OrderItemDto>(orderItem);
                orderItems.Add(mappedOrderItem);
            }
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod, int>().GetByIdAsync(input.DeliveryMethodId);
            if (deliveryMethod is null)
                throw new Exception("delivery Method Not provided");

            var subTotal = orderItems.Sum(item => item.Quantity * item.Price);

            var specs = new OrderWithItemSpecification(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if (existingOrder is null)
                await _paymentService.CreateOrUpdatePaymentIntent(basket);

            var mappedShippingAddress = _mapper.Map<ShippingAddress>(input.ShippingAddress);
            var mappedOrderItems = _mapper.Map<List<OrderItem>>(orderItems);
            var order = new Order
            {
                DeliveryMethodId = deliveryMethod.Id,
                ShippingAddress=mappedShippingAddress,
                BuyerEmail=input.BuyerEmail,
                BasketId=input.BasketId,
                OrderItems=mappedOrderItems,
                SubTotal=subTotal

            };
            try
            {

                await _unitOfWork.Repository<Order, Guid>().AddAsync(order);
                await _unitOfWork.CompleteAsync();

                var mappedOrder = _mapper.Map<OrderDetailsDto>(order);
                return mappedOrder;
            }
            catch (Exception ex) 
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetAllDeliveryMethodAsync()
           => await _unitOfWork.Repository<DeliveryMethod, int>().GetAllAsync();

        public async Task<IReadOnlyList<OrderDetailsDto>> GetAllOrdersForUserAsync(string buyerEmail)
        {
            var specs =  new OrderWithItemSpecification(buyerEmail);
            var orders = await _unitOfWork.Repository<Order, Guid>().GetAlltWithSpecificationAsync(specs);

            if(orders is { Count:<=0})
                throw new Exception("you do not have any orders yet!");

            var mappedOrders = _mapper.Map<List<OrderDetailsDto>>(orders);
            return mappedOrders;
        }
        public async Task<OrderDetailsDto> GetOrderByIdAsync(Guid id,string buyerEmail)
        {
            var specs = new OrderWithItemSpecification(id,buyerEmail);
            var order = await _unitOfWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);
            var mappedOrder = _mapper.Map<OrderDetailsDto>(order);
            return mappedOrder;
        }
    }
}
