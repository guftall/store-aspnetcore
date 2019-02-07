using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShopV1.Core.Interfaces;

namespace OnlineShopV1.Core.Responses
{

    public abstract class Response : IResponse
    {
        public int status { get; set; }
        public string message { get; set; }

        protected Response(StatusCodes status)
        {
            SetStatus(status);
        }
        protected void SetStatus(StatusCodes statusCode)
        {
            status = (int) statusCode;

            message = StatusString(statusCode);
        }

        public static string StatusString(StatusCodes status)
        {
            switch (status)
            {
                // ReSharper disable StringLiteralTypo
                case StatusCodes.Success:
                    return "با موفقیت انجام شد";
                case StatusCodes.BadRequest:
                case StatusCodes.NotFound:
                    return "محتوا پیدا نشد";
                case StatusCodes.InternalError:
                    return "خطای سرور";
                case StatusCodes.AuthenticationFailed:
                case StatusCodes.NotAuthorized:
                case StatusCodes.AuthenticationExpired:
                    return "عدم تایید هویت";
                case StatusCodes.MissMatchPassword:
                    return "رمز قدیمی وارد شده اشتباه است";
                default:
                    return "متن پیام نامشخص است";
                // ReSharper restore StringLiteralTypo
            }
        }
    }


    public class DefaultResponse : Response
    {
        public DefaultResponse(): base(StatusCodes.Success)
        {
        }
        public DefaultResponse(StatusCodes statusCodes): base(statusCodes)
        {
        }
    }

    public class ProductsListResponse : Response
    {
        public List<Product> Products { get; }

        public ProductsListResponse(List<Product> products) : base(StatusCodes.Success)
        {
            Products = products;
        }
    }

    public class ProductResponse : Response
    {
        
        public Product Product { get; }

        public ProductResponse(Product prod) : base(StatusCodes.Success)
        {
            Product = prod;
        }
    }

    public class LoginResponse : Response
    {

        public String AuthCode { get; set; }
        public LoginResponse(string authCode) : base(StatusCodes.Success)
        {
            AuthCode = authCode;
        }
    }

    /* ---------------- Errors ------------- */

    public class MNotFound : Response
    {
        public MNotFound(StatusCodes status = StatusCodes.NotFound) : base(status)
        {
            
        }
    }
    public class ValidationException
    {
        public string Field { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }
    
    public class MBadRequest : Response
    {

        public List<ValidationException> Errors { get; }
        
        public MBadRequest(ModelStateDictionary modelState, StatusCodes status = StatusCodes.BadRequest) : base(status)
        {
            Errors = new List<ValidationException>();
            
            foreach (var (key, value) in modelState)
            {
                var vE = new ValidationException();
                vE.Field = key;
                foreach (var model in value.Errors)
                {
                    vE.Messages.Add(model.ErrorMessage);
                }
                Errors.Add(vE);
            }
        }
    }

    public class MAuthenticationFailed : Response
    {
        public MAuthenticationFailed() : base(StatusCodes.AuthenticationFailed)
        {
            
        }
    }

    public class MAuthenticationExpired : Response
    {
        public MAuthenticationExpired() : base(StatusCodes.AuthenticationExpired)
        {
            
        }
    }
    
    public class InternalErrorResponse : Response
    {
        public InternalErrorResponse(StatusCodes errorCode = StatusCodes.InternalError): base(errorCode)
        {
            
        }
    }



    public enum StatusCodes
    {
        Success=200,
        BadRequest=400,
        MissMatchPassword=4001,
        NotAuthorized=401,
        AuthenticationFailed=4011,
        AuthenticationExpired=4012,
        NotFound=404,
        InternalError=500
    }
}