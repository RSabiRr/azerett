﻿
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Net.Security;
using System.Net;
using System.Xml.Serialization;
using AzerEt.Data;
using AzerEt.Helper;
using x = AzerEt.Helper;
using AzerEt.ViewModels;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace AzerEt.Services
{
    public class PaymentService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PaymentService(IHttpContextAccessor httpContext, AppDbContext context, IWebHostEnvironment env)
        {

            _httpContext = httpContext;
            _context = context;
            _env = env;
        }

        public async Task<string> PayMent(double totalPrice,int id)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string keyPath = Path.Combine(_env.WebRootPath, "Payment", "azeret.pfx");
            string vaultUrl = "https://3dsrv.kapitalbank.az:5443/Exec";
            var check=_context.Checkouts.Where(q=>q.Id==id).FirstOrDefault();
            string xmlSoap = $@"<?xml version='1.0' encoding='UTF-8'?>
                                <TKKPG>
                                    <Request>
                                        <Operation>CreateOrder</Operation>
                                        <Language>AZ</Language>
                                        <Order>
                                            <OrderType>Purchase</OrderType>
                                            <Merchant>E1150036</Merchant>
                                            <Amount>{totalPrice}</Amount>
                                            <Currency>944</Currency>
                                            <Description>xxxxx</Description>
                                            <ApproveURL>https://azeret.az/{"CardCheckTime"}/{check.Id}/{check.UserId} </ApproveURL>
                                            <CancelURL>https://azeret.az/{"cancelurl"}/{check.Id}  </CancelURL>
                                            <DeclineURL>https://azeret.az/{"cancelurl"}/{check.Id} </DeclineURL>
                                        </Order>
                                    </Request>
                                </TKKPG>";

            byte[] pfxRawData = await File.ReadAllBytesAsync(keyPath);

            X509Certificate2 pfxCertWithKey = new X509Certificate2(pfxRawData, "Elxan123@");

            HttpClientHandler handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ClientCertificates = { pfxCertWithKey },
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true; //Todo Test üçünde Əhəd unutma
                }
            };
            using var client = new HttpClient(handler);
            using var content = new StringContent(xmlSoap, Encoding.UTF8, "text/xml");
            //return $"azeret.az/account/login";
            using var request = new HttpRequestMessage(HttpMethod.Post, vaultUrl) { Content = content };

            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return $"https://google.az";
                throw new HttpRequestException($"Error making HTTP request: {response.ReasonPhrase}");
            }

            string result = await response.Content.ReadAsStringAsync();

            var xmlSerializer = new x.XmlSerialaizer();
            TKKPG data = xmlSerializer.Deserialize<TKKPG>(result);


            if (data.Response?.Status == 00)
                return $"https://3dsrv.kapitalbank.az/index.jsp?ORDERID={data.Response.Order.OrderID}&SESSIONID={data.Response.Order.SessionID}";


            throw new Exception("Unexpected payment gateway response.");
        }


        //public  async Task CheckPayment()
        //{
        //    //string payment = _httpContext.HttpContext.Request.Cookies["TKKPG"];

        //    //if (payment == null)
        //    //{
        //    //    throw new Exception();
        //    //}
        //    //_httpContext.HttpContext.Response.Cookies.Delete("TKKPG");
        //    //PaymentVm paymentVm = JsonConvert.DeserializeObject<PaymentVm>(payment);
        //    HttpClient httpClient = new HttpClient();
        //    string key = Path.Combine(_env.WebRootPath, "Payment", "azerettest.pfx");
        //    string vaultUrl = "https://tstpg.kapitalbank.az:5443/Exec";
        //    string xmlSoap = $@"<?xml version='1.0' encoding='UTF - 8'?>
        //    <TKKPG>
        //      <Request>
        //          <Operation>GetOrderStatus</Operation>
        //          <Language>AZ</Language>
        //          <Order><Merchant>E1000010</Merchant><OrderID>  </OrderID></Order>
        //          <SessionID>   </SessionID>
        //      </Request>
        //    </TKKPG>";
        //    byte[] pfxRawData = File.ReadAllBytes(key);
        //    using (X509Certificate2 pfxCertWithKey = new X509Certificate2(pfxRawData, "Elxan123@"))
        //    {
        //        HttpClientHandler handler = new HttpClientHandler();
        //        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        //        handler.ServerCertificateCustomValidationCallback = (a, b, c, d) => { return true; };
        //        handler.ClientCertificates.Add(pfxCertWithKey);
        //        using (HttpContent content = new StringContent(xmlSoap, Encoding.UTF8, "text/xml"))
        //        using (var client = new HttpClient(handler))
        //        using (var request = new HttpRequestMessage(HttpMethod.Post, vaultUrl))
        //        {
        //            request.Headers.Add("SOAPAction", "");
        //            request.Content = content;
        //            var response = client.SendAsync(request).GetAwaiter().GetResult();
                   
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                throw new HttpRequestException($"Error making HTTP request: {response.ReasonPhrase}");
        //            }

        //            var result = await response.Content.ReadAsStringAsync();

        //            XmlSerialaizer xmlSerialaizer = new XmlSerialaizer();
        //            var data = xmlSerialaizer.Deserialize<TKKPG>(result);

        //            if (data.Response?.Status == 00)
        //            {
        //                throw new Exception("Unexpected payment gateway response.");
        //            }
        //        }

        //    }
           
             
           
        //}
    }
}