using BlazorWasm.ExpenseTracker.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Transactions;

namespace BlazorWasm.ExpenseTracker.Pages
{
    public partial class Index
    {
        private string Amount = string.Empty;

        private TransactionModel transaction = new TransactionModel();
        private List<TransactionModel> transactions = new List<TransactionModel>();

        protected override void OnInitialized()
        {
            transaction.TransactionType = "Expense";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await List();
                await _jsRuntime.InvokeVoidAsync("handleKeyPress", "amount");
            }
        }

        async Task AddTransaction()
        {
            //var data = await _jsRuntime.InvokeAsync<string>("getTextboxValue", "amount");
            //transaction.Amount = Convert.ToDecimal(data.ToString());
            transaction.Amount = Convert.ToDecimal(Amount);
            transaction.CreatedDateTime = DateTime.Now;
            transaction.ModifiedDateTime = DateTime.Now;
            await List();
            transaction.Id = transactions.Count == 0 ? 1 : transactions.Max(x => x.Id) + 1;
            transactions.Add(transaction);
            await _jsRuntime.InvokeAsync<string>("localStorage.setItem", "Transactions", JsonConvert.SerializeObject(transactions));
            Amount = string.Empty;
            transaction = new TransactionModel();
        }

        async Task RemoveTransaction(int id)
        {
            await List();
            var item = transactions.FirstOrDefault(x => x.Id == id);
            if (item == null) return;
            transactions.Remove(item);
            await _jsRuntime.InvokeAsync<string>("localStorage.setItem", "Transactions", JsonConvert.SerializeObject(transactions));
            await List();
        }

        async Task List()
        {
            var jsonStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "Transactions");
            if (jsonStr != null)
            {
                transactions = JsonConvert.DeserializeObject<List<TransactionModel>>(jsonStr);
                StateHasChanged();
            }
        }

        void TransactionTypeChange(ChangeEventArgs args)
        {
            transaction.TransactionType = args.Value.ToString();
        }

        private string Total => (transactions.Where(x => x.TransactionType == "Income").Sum(x => x.Amount) +
            (transactions.Where(x => x.TransactionType == "Expense").Sum(x => x.Amount) * -1)).ToString("n2");

        private string Income => transactions.Where(x => x.TransactionType == "Income").Sum(x => x.Amount).ToString("n2");

        private string Expense => transactions.Where(x => x.TransactionType == "Expense").Sum(x => x.Amount).ToString("n2");
    }
}
