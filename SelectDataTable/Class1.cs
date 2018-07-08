using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectDataTable
{
    public class Class1
    {
        public void Hello()
        {
            Cat cat = new Cat();

            Foo(cat);
        }
        public void Foo<T>(T t) where T: IAnimal
        {

        }
    }

    public class Cat : IAnimal
    {

        public void Bite()
        {
            throw new NotImplementedException();
        }
    }

    public class Dog : IAnimal
    {

        public void Bite()
        {
            throw new NotImplementedException();
        }
    }

    public interface IAnimal
    {
        void Bite();
    }
}
