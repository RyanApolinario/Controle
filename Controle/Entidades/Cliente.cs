using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;

namespace Controle
{
    public class Cliente
    {
        public Cliente()
        {

        }
        [Display(Name = "id")]
        [Column("id")]
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Column("Nome")]
        public string Nome { get; set; }

        [Display(Name = "Documento")]
        [Column("Documento")]
        public string Documento { get; set; }
    }
}