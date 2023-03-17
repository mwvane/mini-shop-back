using mini_shop_api.Models;

namespace mini_shop_api.Helpers
{
    public static class Helper
    {
        public static OrderFront ConvertOrderForFront(Order product, MyDbContext context)
        {
            return new OrderFront()
            {
                Id = product.Id,
                Product = context.Products.Where(item => item.Id == product.ProductId).FirstOrDefault(),
                Quantity = product.Quantity,
                TotalPrice = product.TotalPrice,
                User = context.Users.Where(item => item.Id == product.UserId).FirstOrDefault(),
                Voucher = context.Vouchers.Where(item => item.Id == product.VoucherId).FirstOrDefault(),
                CreateDate = product.CreateDate,
                VoucherAmount = product.VoucherAmount,
            };
        }
        public static Order ConvertOrderForBack(OrderFront product, MyDbContext context)
        {
            return new Order()
            {
                Id = product.Id,
                ProductId = product.Product.Id,
                Quantity = product.Quantity,
                TotalPrice = product.TotalPrice,
                UserId = product.User.Id,
                VoucherId = product.Voucher.Id,
                CreateDate = product.CreateDate,
                VoucherAmount = product.VoucherAmount,
            };
        }

        public static CartItemFront ConvertCartItemForFront(CartItem cartItem, MyDbContext context)
        {
            return new CartItemFront()
            {
                Id = cartItem.Id,
                Product = ConvertProductForFront(context.Products.Where(item => item.Id == cartItem.ProductId).FirstOrDefault(), context),
                Quantity = cartItem.Quantity,
                TotalPrice = cartItem.TotalPrice,
                User = context.Users.Where(item => item.Id == cartItem.UserId).FirstOrDefault(),
                Voucher = context.Vouchers.Where(item => item.Id == cartItem.VoucherId).FirstOrDefault(),
                VoucherAmount = cartItem.VoucherAmount = cartItem.VoucherAmount,
            };
        }

        public static CartItem ConvertCartItemForBack(CartItemFront cartItemFront, MyDbContext context)
        {
            return new CartItem()
            {
                Id = cartItemFront.Id,
                ProductId = cartItemFront.Product.Id,
                Quantity = cartItemFront.Quantity,
                TotalPrice = cartItemFront.TotalPrice,
                UserId = cartItemFront.User.Id,
                VoucherId = cartItemFront.Voucher == null ? null : cartItemFront.Voucher.Id,
                VoucherAmount = cartItemFront.VoucherAmount,

            };
        }
        public static ProductFront ConvertProductForFront(Product product, MyDbContext context)
        {
            List<string> productImageUrls = new List<string>();
            List<string> productDocumentUrls = new List<string>();
            foreach (var item in context.ProductImages)
            {
                if (item.ProductId == product.Id)
                {
                    productImageUrls.Add(Constants.DEFAULT_SERVER + item.ImageUrl);
                }
            }
            foreach (var item in context.ProductDocuments)
            {
                if (item.ProductId == product.Id)
                {
                    productDocumentUrls.Add(Constants.DEFAULT_SERVER + item.DocumentUrl);
                }
            }
            return new ProductFront()
            {
                Id = product.Id,
                CreatedBy = product.CreatedBy,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity,
                ImageUrls = productImageUrls,
                DocumentUrls = productDocumentUrls,
            };
        }
    }
}
