using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Description;
using wrsProjetoFinalDM106.br.com.correios.ws;
using wrsProjetoFinalDM106.CRMclient;
using wrsProjetoFinalDM106.Models;

namespace wrsProjetoFinalDM106.Controllers
{
    [RoutePrefix("api/Orders")]
    [Authorize]
    public class OrdersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public List<Order> GetOrders()
        {
            return db.Orders.Include(order => order.OrderItems).ToList();
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return Content(HttpStatusCode.NoContent, "Nao existe nenhum pedido com o id: " + id);
            }

            if (!checkAccessPermition(User, order.userEmail))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            return Ok(order);
        }

        // GET: api/Orders/email
        [ResponseType(typeof(List<Order>))]
        [HttpGet]
        [Route("byemail")]
        public IHttpActionResult GetOrder(string email)
        {
            var orders = db.Orders.Where(o => o.userEmail == email);
            if (orders == null)
            {
                return Content(HttpStatusCode.NoContent, "Nao existe pedidos cadastrados no email: " + email);
            }

            if (!checkAccessPermition(User, email))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            return Ok(orders.ToList());
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return Content(HttpStatusCode.NoContent, "Nao existe nenhum pedido com o id: " + id);
            }

            if (!checkAccessPermition(User, order.userEmail))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (order.freightPrice == 0)
            {
                return Content(HttpStatusCode.Forbidden, "Calcule o frete antes de fechar o pedido.");
            }

            order.status = "Fechado";

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!checkAccessPermition(User, order.userEmail))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            order.status = "Novo";
            order.orderWeight = 0;
            order.freightPrice = 0;
            order.orderPrice = 0;
            order.orderDate = DateTime.Now;
            order.deliveryDate = DateTime.Now;

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return Content(HttpStatusCode.NoContent, "Nao existe nenhum pedido com o id: " + id);
            }

            if (!checkAccessPermition(User, order.userEmail))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("frete")]
        public IHttpActionResult CalculaFrete(int id)
        {
            string frete;
            string cep;

            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return Content(HttpStatusCode.NoContent, "Nao existe nenhum pedido com o id: " + id);
            }

            if (!checkAccessPermition(User, order.userEmail))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (order.OrderItems.Count <= 0)
            {
                return Content(HttpStatusCode.NoContent, "Pedido sem itens");
            }

            if (!order.status.Equals("Novo"))
            {
                return Content(HttpStatusCode.NotAcceptable, "Pedido com status diferente de “novo”");
            }

            cep = ObtemCEP(User);
            if (cep == null)
            {
                return Content(HttpStatusCode.NotFound, "Impossibilidade de acessar o serviço de CRM");
            }

            Product total = calcInfoFrete(order);

            CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
            cResultado resultado = correios.CalcPrecoPrazo("", "", "40010", "37540000", cep, Convert.ToString(total.peso), 1, total.comprimento, total.altura, total.largura, total.diamentro, "N", 0, "N");
            if (resultado.Servicos[0].Erro.Equals("0"))
            {
                frete = "Valor do frete: " + resultado.Servicos[0].Valor + " - Prazo de entrega: " + resultado.Servicos[0].PrazoEntrega + " dia(s)";

                try
                {
                    order.freightPrice = Convert.ToDecimal(resultado.Servicos[0].Valor);
                    order.orderPrice += order.freightPrice;
                    order.deliveryDate = order.orderDate.AddDays(Convert.ToInt16(resultado.Servicos[0].PrazoEntrega));
                }
                catch
                {
                    return Content(HttpStatusCode.InternalServerError, "Erro na resposta do serviço dos Correios");
                }
                return Ok(frete);
            }
            else
            {
                return Content(HttpStatusCode.NotFound, "Impossibilidade de acessar o serviço dos Correios. Código do erro: " + resultado.Servicos[0].Erro + " - " + resultado.Servicos[0].MsgErro);
            }
        }

        private Product calcInfoFrete(Order order)
        {
            Product totalPackage = new Product();

            totalPackage.altura = 0;
            totalPackage.comprimento = 0;
            totalPackage.diamentro = 0;
            totalPackage.largura = 0;
            totalPackage.peso = 0;

            foreach (OrderItem item in order.OrderItems)
            {
                totalPackage.altura += item.Product.altura;
                totalPackage.comprimento += item.Product.comprimento;
                totalPackage.diamentro += item.Product.diamentro;
                totalPackage.largura += item.Product.largura;
                totalPackage.peso += item.Product.peso;
            }

            return totalPackage;
        }

        private string ObtemCEP(IPrincipal user)
        {
            CRMRestClient crmClient = new CRMRestClient();
            Customer customer = crmClient.GetCustomerByEmail(user.Identity.Name);
            if (customer != null)
            {
                return customer.zip;
            }
            else
            {
                return null;
            }
        }

        private bool checkAccessPermition(IPrincipal user, string email)
        {
            return ((user.Identity.Name.Equals(email)) || (user.IsInRole("ADMIN")));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}