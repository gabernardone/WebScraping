using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;


namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {


            var baseUrl = "http://www.freitasleiloesonline.com.br/";

            var client = new HtmlWeb();

            var paginaMateriaisHome = client.Load(baseUrl + "/homesite/filtro.asp?q=materiais");

            var codigoUltimaPagina = paginaMateriaisHome.DocumentNode.SelectNodes("//*[@id='listaLotesPaginacao']/ul/li").Count;

            Console.WriteLine(codigoUltimaPagina);

            //No lugar do dois, seria idela colocar o código da última página.
            for (int i = 1; i <= 2 ; i++)
            {

                var paginaMateriais = client.Load(baseUrl + "/homesite/filtro.asp?q=materiais&txBuscar=&CodSubCategoria=&SubCategoria=&UF=&CodCondicao=&Condicao=&OptValores=0&LblValores=&pagina=" + i);

                var materiais = paginaMateriais.DocumentNode.SelectNodes("//div[@id='listaLotes']/ul/li");

                var ListaMateriais = new List<Material>();


                foreach (var item in materiais)
                {
                    var urlImagem = item.SelectSingleNode("div[1]/img").Attributes["src"].Value;

                    //Remover o elemento H1. Pesquisa primeiro e depois remove.
                    item.SelectSingleNode("div[2]/h1").Remove();


                    //Com o InnerHtml conseguimos pegar o texto dentro do elemento (no caso a DIV).
                    var descricao = item.SelectSingleNode("div[2]").InnerHtml.Trim();



                    //Pega o valor que esta entro da div(lance inicial : R$ 80,00) e quebrar em um arrey
                    //para isso vamos utilizar o split, e como sabemos que vão virar 2 itens, vamos pegar
                    // o último item do arrey com o Last
                    var lanceInicial = item.SelectSingleNode("div[3]/div[1]").InnerHtml.Split(':').Last().Trim();

                    var maiorLance = item.SelectSingleNode("div[3]/div[2]").InnerHtml.Split(':').Last().Trim();

                    var quantidadeLances = item.SelectSingleNode("div[3]/div[3]").InnerHtml.Split(':').Last().Trim();

                    var materialMOD = new Material();

                    materialMOD.urlFoto = urlImagem;
                    materialMOD.descricao = descricao;
                    materialMOD.lanceInicial = lanceInicial;
                    materialMOD.maiorLance = maiorLance;
                    materialMOD.quantidadeLances = Convert.ToInt32(quantidadeLances);

                    ListaMateriais.Add(materialMOD);

                }

                using (var conexao = new LEILAOEntities())
                {
                    conexao.Material.AddRange(ListaMateriais);
                    conexao.SaveChanges();
                }

                Console.WriteLine("Dados importados para o Banco de dados");
                    
            }
        

            Console.ReadKey();

        }
    }
}
