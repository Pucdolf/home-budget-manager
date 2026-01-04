using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetManager.Core
{
    internal class Category
    {
        private int id;
        private String name;
        private String? description;
        
        public Category(String name)
        {
            this.id = 0;
            this.name = name;
            this.description = null;
        }

        public Category(String name, String description)
        {
            //add int id generator
            this.id = 0;
            this.name = name;
            this.description = description;

        }
         
        public Category(int id, String name, String description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        public int getId() { return id; }
        public String getName() { return name; }
        public String? getDescription() { return description; }

        public void changeName(String name)
        {
            this.name = name;
        } 

        public void changeDescription(String description)
        {
            this.description= description;
        }
    }
}
