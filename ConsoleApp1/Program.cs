using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnreflectedSerializer
{
	public delegate void GetStringRepresentation<T>(T instance, StringBuilder builder);

	public class RootDescriptor<T>
	{
		public GetStringRepresentation<T>[] FuctionsDescribingThis;

		public RootDescriptor()
		{
			this.FuctionsDescribingThis = new GetStringRepresentation<T>[0];
		}
		public RootDescriptor(GetStringRepresentation<T>[] delegates)
		{
			this.FuctionsDescribingThis = delegates;
		}

		public void Serialize(TextWriter writer, T instance)
		{
			StringBuilder builder = new StringBuilder();
			Serialize(instance, typeof(T).Name, builder);
			writer.Write(builder.ToString());
		}

		public void Serialize(T instance, string instanceName, StringBuilder builder)
		{
			if (this.FuctionsDescribingThis.Length == 0)
			{
				builder.Append($"<{instanceName}>{instance}</{instanceName}>\n");
			}
			else
			{
				builder.Append($"<{instanceName}>\n");
				foreach (var GetStringRep in this.FuctionsDescribingThis)
				{
					GetStringRep(instance, builder);
				}
				builder.Append($"</{instanceName}>\n");
			}
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
			GetStringRepresentation<Person> FirstName = new GetStringRepresentation<Person>((Person person, StringBuilder builder) =>
				{  GetStringDescriptor().Serialize(person.FirstName, nameof(person.FirstName), builder); });
			GetStringRepresentation<Person> LastName = new GetStringRepresentation<Person>((Person person, StringBuilder builder ) =>
				{  GetStringDescriptor().Serialize(person.LastName, nameof(person.LastName), builder); });
			GetStringRepresentation<Person> HomeAddress = new GetStringRepresentation<Person>((Person person, StringBuilder builder) =>
			    {  GetAddressDescriptor().Serialize(person.HomeAddress, nameof(person.HomeAddress), builder); });
			GetStringRepresentation<Person> WorkAddress = new GetStringRepresentation<Person>((Person person, StringBuilder builder) =>
				{  GetAddressDescriptor().Serialize(person.WorkAddress, nameof(person.WorkAddress), builder); });
			GetStringRepresentation<Person> CitizenOf = new GetStringRepresentation<Person>((Person person, StringBuilder builder) =>
				{  GetCountryDescriptor().Serialize(person.CitizenOf, nameof(person.CitizenOf), builder); });
			GetStringRepresentation<Person> MobilePhone = new GetStringRepresentation<Person>((Person person, StringBuilder builder) =>
				{  GetPhoneNumberDescriptor().Serialize(person.MobilePhone, nameof(person.MobilePhone), builder); });

			RootDescriptor<Person> rootDesc = new RootDescriptor<Person>(new GetStringRepresentation<Person>[]
			{FirstName, LastName, HomeAddress, WorkAddress, CitizenOf, MobilePhone });
			
			return rootDesc;
		}

		static RootDescriptor<int> GetIntDescriptor()
		{
			return new RootDescriptor<int>(); //instance of root descriptor with an empty delegate list
		}
		static RootDescriptor<string> GetStringDescriptor()
		{
			return new RootDescriptor<string>();
		}
		static RootDescriptor<Address> GetAddressDescriptor()
		{
			GetStringRepresentation<Address> Street = new GetStringRepresentation<Address>((Address adr, StringBuilder builder) =>
				{ GetStringDescriptor().Serialize(adr.Street, nameof(adr.Street), builder); });
			GetStringRepresentation<Address> City = new GetStringRepresentation<Address>((Address adr, StringBuilder builder) =>
				{ GetStringDescriptor().Serialize(adr.City, nameof(adr.City), builder) ; });

			return new RootDescriptor<Address>(new GetStringRepresentation<Address>[] { Street, City });
		}
		static RootDescriptor<Country> GetCountryDescriptor()
		{
			GetStringRepresentation<Country> Name = new GetStringRepresentation<Country>((Country country,StringBuilder builder) =>
				{ GetStringDescriptor().Serialize(country.Name, nameof(country.Name), builder); });
			GetStringRepresentation<Country> AreaCode = new GetStringRepresentation<Country>((Country country, StringBuilder builder) =>
				{ GetIntDescriptor().Serialize(country.AreaCode, nameof(country.AreaCode), builder); });

			return new RootDescriptor<Country>(new GetStringRepresentation<Country>[] { Name, AreaCode });
		}
		static RootDescriptor<PhoneNumber> GetPhoneNumberDescriptor()
		{
			GetStringRepresentation<PhoneNumber> Country = new GetStringRepresentation<PhoneNumber>((PhoneNumber phoneNumber, StringBuilder builder) =>
				{ GetCountryDescriptor().Serialize(phoneNumber.Country, nameof(phoneNumber.Country), builder); });
			GetStringRepresentation<PhoneNumber> Number = new GetStringRepresentation<PhoneNumber>((PhoneNumber phoneNumber, StringBuilder builder) =>
				{ GetIntDescriptor().Serialize(phoneNumber.Number, nameof(phoneNumber.Number),builder ); });

			return new RootDescriptor<PhoneNumber>(new GetStringRepresentation<PhoneNumber>[] { Country, Number });
		}
	}
}
