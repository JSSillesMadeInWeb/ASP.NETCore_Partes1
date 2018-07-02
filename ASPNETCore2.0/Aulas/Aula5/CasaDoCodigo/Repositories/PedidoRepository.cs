using CasaDoCodigo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public interface IPedidoRepository
    {
        Pedido GetPedido();
        void AddItem(string codigo);
    }

    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {
        private readonly IHttpContextAccessor contextAccessor;

        public PedidoRepository(ApplicationContext contexto,
            IHttpContextAccessor contextAccessor) : base(contexto)
        {
            this.contextAccessor = contextAccessor;
        }

        public void AddItem(string codigo)
        {
            var produto = contexto.Set<Produto>()
                            .Where(p => p.Codigo == codigo)
                            .SingleOrDefault();

            //se não tem nenhum produto
            if (produto == null)
            {
                throw new ArgumentException("Produto não encontrado");
            }

            var pedido = GetPedido();

            var itemPedido = contexto.Set<ItemPedido>()
                                .Where(i => i.Produto.Codigo == codigo
                                        && i.Pedido.Id == pedido.Id)
                                .SingleOrDefault();
            //se não foi encontrada o item de pedido,
            //temos que adicionar ao carrinho
            if (itemPedido == null)
            {
                itemPedido = new ItemPedido(pedido, produto, 1, produto.Preco);
                contexto.Set<ItemPedido>()
                    .Add(itemPedido);

                contexto.SaveChanges();
            }
        }

        public Pedido GetPedido()
        {
            //para obter o pedido atual
            var pedidoId = GetPedidoId();

            //verificar se existe um pedido,
            //caso não exista, ele cria um novo
            //vendo se existem atraves de expressões lambda,
            //utilizando tambem a clausula Where, parecida com a do SQL

            /*SingleOrDefault: Se encontrar um elemento,  retorna
              Caso contrário, retorna null*/
            /*Include -> especificando a entidade que estamos incluindo*/
            var pedido = dbSet.Include(p => p.Itens).ThenInclude(i => i.Produto)
                .Where(p => p.Id == pedidoId).SingleOrDefault();

            if (pedido == null)
            {
                pedido = new Pedido();
                //add ao dbSet de pedidos
                dbSet.Add(pedido);
                //salvando no banco
                contexto.SaveChanges();
                SetPedidoId(pedido.Id);
            }

            //caso o pedido tenha alguma informação
            return pedido;
        }

        private int? GetPedidoId()
        {
            return contextAccessor.HttpContext.Session.GetInt32("pedidoId");
        }

        private void SetPedidoId(int pedidoId)
        {
            contextAccessor.HttpContext.Session.SetInt32("pedidoId", pedidoId);
        }
    }
}