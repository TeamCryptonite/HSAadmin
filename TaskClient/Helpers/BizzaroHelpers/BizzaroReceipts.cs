﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HsaServiceDtos;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace TaskClient.Helpers.BizzaroHelpers
{
    public class BizzaroReceipts : AbstractBizzaroActions
    {
        public BizzaroReceipts(AuthenticationResult authenticationResult) : base(authenticationResult)
        {
        }

        public Paginator<ReceiptDto> GetListOfReceipts(string query = null)
        {
            var request = new RestRequest("receipts", Method.GET);
            if (!string.IsNullOrWhiteSpace(query))
                request.AddQueryParameter("query", query);

            return new Paginator<ReceiptDto>(this, request);
        }

        public async Task<ReceiptDto> GetOneReceipt(int receiptId)
        {
            if (receiptId < 1)
                throw new Exception("Receipt ID must be greater than 0.");
            var request = new RestRequest($"receipts/{receiptId}", Method.GET);

            return await CallBizzaro<ReceiptDto>(request);
        }

        public async Task<ReceiptDto> PostNewReceipt(ReceiptDto receiptDto)
        {
            var request = new RestRequest("receipts", Method.POST);

            return await CallBizzaro<ReceiptDto>(request, receiptDto);
        }

        public async Task<StatusOnlyDto> UpdateReceipt(int receiptId, ReceiptDto updatedReceipt)
        {
            var request = new RestRequest("receipts/{id}", Method.PATCH);
            request.AddUrlSegment("id", receiptId.ToString());

            var status = await CallBizzaro(request, updatedReceipt);

            return status;
        }

        public async Task<StatusOnlyDto> DeleteReceipt(int receiptId)
        {
            var request = new RestRequest($"receipts/{receiptId}", Method.DELETE);

            return await CallBizzaro(request);
        }

        public async Task<LineItemDto> AddReceiptListItem(int receiptId,
            LineItemDto newReceiptListItem)
        {
            var request = new RestRequest($"receipts/{receiptId}/lineitems", Method.POST);

            return await CallBizzaro<LineItemDto>(request, newReceiptListItem);
        }

        public async Task<StatusOnlyDto> DeleteReceiptListItem(int receiptId, int receiptListItemId)
        {
            var request = new RestRequest($"receipts/{receiptId}/lineitems/{receiptListItemId}",
                Method.DELETE);

            return await CallBizzaro(request);
        }

        public async Task<StatusOnlyDto> UpdateReceiptListItem(int receiptId,
            LineItemDto updatedLineItem)
        {
            if (updatedLineItem.LineItemId < 1)
                return new StatusOnlyDto { StatusMessage = "Receipt Line Items must include an ID" };
            var request =
                new RestRequest(
                    $"receipts/{receiptId}/lineitems/{updatedLineItem.LineItemId}",
                    Method.PATCH);

            return await CallBizzaro(request, updatedLineItem);
        }
    }
}