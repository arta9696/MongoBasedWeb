using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoBasedWeb.Models;

namespace MongoBasedWeb.Data
{
    public class MongoBasedWebContext
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CatsCollectionName { get; set; } = null!;
        public string AttributesCollectionName { get; set; } = null!;
    }
}
