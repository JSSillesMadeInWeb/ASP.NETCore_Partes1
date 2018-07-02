using CasaDoCodigo.Models;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/* Alterações 02/07/18 */
namespace CasaDoCodigo.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IProdutoRepository produtoRepository;
        private readonly IPedidoRepository pedidoRepository;

        public PedidoController(IProdutoRepository produtoRepository,
            IPedidoRepository pedidoRepository)
        {
            this.produtoRepository = produtoRepository;
            this.pedidoRepository = pedidoRepository;
        }

        public IActionResult Carrossel()
        {
            return View(produtoRepository.GetProdutos());
        }

        public IActionResult Carrinho(string codigo)
        {   
            /*
              O código abaixo, faz com que o pedido seja criado duas vezes
              
              var pedidoId - GetPedidoId();
              var pedido = dbSet.Where(p=> p.Id == pedidoId)
                                            .SingleOrDefault();

             if(pedido == null)
             {
                pedido = new Pedido();
                dbSet.Add(pedido);
                contexto.SaveChanges();
             }
            */

            //verificar se o pedido, foi ou não, preenchido
            if (!string.IsNullOrEmpty(codigo))
            {
                //add o item no repositório de pedidos
                pedidoRepository.AddItem(codigo);
            }

            //criando um novo pedido
            Pedido pedido = pedidoRepository.GetPedido();
            return View(pedido.Itens);
        }

        public IActionResult Cadastro()
        {
            return View();
        }

        public IActionResult Resumo()
        {
            Pedido pedido = pedidoRepository.GetPedido();
            return View(pedido);
        }

    }
}