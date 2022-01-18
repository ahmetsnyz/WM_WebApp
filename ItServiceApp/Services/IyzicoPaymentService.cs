﻿using AutoMapper;
using ItServiceApp.Models.Payment;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ItServiceApp.Services
{
    public class IyzicoPaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IyzicoPaymentOptions _options;
        private readonly IMapper _mapper;
        public IyzicoPaymentService(IConfiguration configuration, IyzicoPaymentOptions options,IMapper mapper)
        {
            _configuration = configuration;
            _options = options;
            _mapper = mapper;
            var section = _configuration.GetSection(IyzicoPaymentOptions.Key);
            _options = new IyzicoPaymentOptions()
            {
                ApiKey = section["ApiKey"],
                BaseUrl = section["BaseUrl"],
                SecretKey = section["SecretKey"],
                ThreedsCallbackUrl = section["ThreedsCallbackUrl"]
            };
        }
        private string GenerateConversationId()
        {
            return MUsefulMethods.StringHelpers.GenerateUniqueCode();
        }
        public InstalmentModel CheckInstallments(string binNumber, decimal price)
        {
            if (binNumber.Length > 6)
            {
                binNumber = binNumber.Substring(0, 6);
            }
            var conversationId = GenerateConversationId();
            var request = new RetrieveInstallmentInfoRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = conversationId,
                BinNumber = binNumber,
                Price = price.ToString(new CultureInfo("en-US"))
            };
            var result = InstallmentInfo.Retrieve(request, _options);
            if (result.Status == "failure")
            {
                throw new Exception(result.ErrorMessage);
            }
            if (result.ConversationId != conversationId)
            {
                throw new Exception("Hatalı istek oluşturuldu");
            }
            var resultModel = _mapper.Map<InstalmentModel>(result.InstallmentDetails[0]);
            return resultModel;
        }

        public PaymentResponseModel Pay(PaymentModel model)
        {
            throw new NotImplementedException();
        }
    }
}