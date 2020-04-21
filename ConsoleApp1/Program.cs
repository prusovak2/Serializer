using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnreflectedSerializer
{
	/// <summary>
	/// delegate to function appending a string reprsentation of T instance to StringBuilder builder
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="instance"></param>
	/// <param name="builder"></param>
	public delegate void GetStringRepresentation<T>(T instance, StringBuilder builder);
	/// <summary>
	/// SERIALIZER
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RootDescriptor<T>
	{
		/// <summary>
		/// array containig delegates to functions returnig a string representation of fields of object described by this descriptor
		/// </summary>
		public GetStringRepresentation<T>[] FuctionsDescribingThis;
		/// <summary>
		/// to construct desciptors simple types - int string...
		/// empty delegate array indicates a simple type
		/// </summary>
		public RootDescriptor()
		{
			this.FuctionsDescribingThis = new GetStringRepresentation<T>[0];
		}
		/// <summary>
		/// to construct desciptors for compound types
		/// </summary>
		public RootDescriptor(GetStringRepresentation<T>[] delegates)
		{
			this.FuctionsDescribingThis = delegates;
		}
		/// <summary>
		/// wrapper around my own serialize, dealing with a StringBuilder
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="instance"></param>
		public void Serialize(TextWriter writer, T instance)
		{
			StringBuilder builder = new StringBuilder();
			Serialize(instance, typeof(T).Name, builder);
			writer.Write(builder.ToString());
		}
		/// <summary>
		/// fills a stringbuilder with a string representation of T instance
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="instanceName"></param>
		/// <param name="builder"></param>
		public void Serialize(T instance, string instanceName, StringBuilder builder)
		{
			if (this.FuctionsDescribingThis.Length == 0)
			{
				//simple value
				builder.Append($"<{instanceName}>{instance}</{instanceName}>\n");
			}
			else
			{
				//compound value
				builder.Append($"<{instanceName}>\n");
				foreach (var GetStringRep in this.FuctionsDescribingThis)
				{
					GetStringRep(instance, builder); //function hidden in FunctionDescribingThis call serialize() - hidden recursion
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
		//Follow a set of methods serving as a configuration for a serializer

		/// <summary>
		/// returns a description of Person 
		/// </summary>
		/// <returns></returns>
		static RootDescriptor<Person> GetPersonDescriptor()
		{
			//Create delegates to functions describing Person fields
			//functions are to be called from the Serialize function
			//descritors for each field nested anywhere in Person object is recursivelly created and .Serialize() is called on each descriptor
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

			//create new person descriptor with coresponding array of delegates
			RootDescriptor<Person> rootDesc = new RootDescriptor<Person>(new GetStringRepresentation<Person>[]
			{FirstName, LastName, HomeAddress, WorkAddress, CitizenOf, MobilePhone });
			
			return rootDesc;
			//no Person instance is passed directly to GetDescriptor() functions - Descriptors can be created independentally of a particular instance
			//Person instance is passed to Serialize function when called, then propageted thru corresponding delegates to recursive serialize calls
		}

		//Descriptors for simple types, string and int is currently supported
		//to support any other simple type, add corresponding GetDescrtiptor method
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
