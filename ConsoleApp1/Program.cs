using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnreflectedSerializer
{
	public delegate string GetFieldName();
	public delegate string GetValue();
	public delegate void FillValues<T>(T instance);

	public class RootDescriptor<T>
	{
		public FillValues<T> fillValues;
		public List<GetFieldName> getFieldNames = new List<GetFieldName>();
		public List<GetValue> getValues = new List<GetValue>();

		public void Serialize(TextWriter writer, T instance)
		{
			
		}
	}

	class Address
	{
		public string Street { get; set; }
		public string City { get; set; }
	}

	class Country
	{
		public string Name { get; set; }
		public int AreaCode { get; set; }
	}

	class PhoneNumber
	{
		public Country Country { get; set; }
		public int Number { get; set; }
	}

	class Person
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public Address HomeAddress { get; set; }
		public Address WorkAddress { get; set; }
		public Country CitizenOf { get; set; }
		public PhoneNumber MobilePhone { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			RootDescriptor<Person> rootDesc = GetPersonDescriptor();

			var czechRepublic = new Country { Name = "Czech Republic", AreaCode = 420 };
			var person = new Person
			{
				FirstName = "Pavel",
				LastName = "Jezek",
				HomeAddress = new Address { Street = "Patkova", City = "Prague" },
				WorkAddress = new Address { Street = "Malostranske namesti", City = "Prague" },
				CitizenOf = czechRepublic,
				MobilePhone = new PhoneNumber { Country = czechRepublic, Number = 123456789 }
			};

			rootDesc.Serialize(Console.Out, person);
		}

		static RootDescriptor<Person> GetPersonDescriptor()
		{
			var rootDesc = new RootDescriptor<Person>();

			List<GetFieldName> getFieldNames = new List<GetFieldName>();
			List<GetValue> getValues = new List<GetValue>();

			PersonDescriptor personDescriptor = new PersonDescriptor();

			rootDesc.fillValues = new FillValues<Person>(personDescriptor.FillValues);

			return rootDesc;
		}
	}
}
