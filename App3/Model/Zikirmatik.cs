using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App3.Model
{
    [Table("ZikirmatikSayac")]
    public class Zikirmatik
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Counter { get; set; }
    }
}
