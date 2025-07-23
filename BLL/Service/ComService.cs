using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.AspNetCore.SignalR.Client;

namespace BLL.Service
{
    public class ComService : IComService
    {
        private readonly IComRepo _repo;
        private static HubConnection? _connection;
        private static bool _connected = false;
        public ComService(IComRepo repo)
        {
            _repo = repo;
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7082/DataSignalRChanel") // RazorPage Host

        .WithAutomaticReconnect()
        .Build();
        }

        public List<Comment> GetProductComments(int productId) => _repo.GetVisibleCommentsByProduct(productId);

        public void CreateComment(Comment comment) => _repo.AddComment(comment);

        public void UpdateComment(Comment comment) => _repo.UpdateComment(comment);

        public void HideComment(int commentId) => _repo.SoftDeleteComment(commentId);

        public Comment GetById(int id) => _repo.GetCommentById(id);

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            return await _repo.GetAllCommentsAsync();
        }
        public async Task NotifyLoadAsync()
        {
            if (!_connected || _connection.State != HubConnectionState.Connected)
            {
                await _connection.StartAsync();
                _connected = true;
            }

            await _connection.InvokeAsync("SendAllLoadComment");
        }
    }

}
