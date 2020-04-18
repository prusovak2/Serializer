using System;
using System.Collections.Generic;
using System.Text;

namespace UnreflectedSerializer
{
    class PersonDescriptor
    {
        private Person person { get; set; }
        public string GetFirstNameFieldName => "FirstName";
        public string GetFirstName => this.person.FirstName;
        public string GetLastNameFieldName => "LastName";
        public string GetLasttName => this.person.LastName;
        public string GetHomeAdressFieldName => "HomeAddress";
        public AddressDescriptor HomeAddress { get; set; }
        public string GetWorkAdressFieldName => "WorkAddress";
        public AddressDescriptor WorkAddress { get; set; }
        public string GetCitizenOfFieldName => "CitizenOf";
        public CountryDescriptor CitizenOf { get; set; }
        public string GetMobilePhoneFieldName => "MobilePhone";
        public PhoneNumberDescriptor MobilePhone { get; set; }

        public int NumOfFields = 6;

        public PersonDescriptor()
        {
            this.HomeAddress = new AddressDescriptor();
            this.WorkAddress = new AddressDescriptor();
            this.CitizenOf = new CountryDescriptor();
            this.MobilePhone = new PhoneNumberDescriptor();
        }
        public void FillValues(Person person)
        {
            this.person = person;
            this.HomeAddress.FillValues(person.HomeAddress);
            this.WorkAddress.FillValues(person.WorkAddress);
            this.CitizenOf.FillValues(person.CitizenOf);
            this.MobilePhone.FillValues(person.MobilePhone);
        }

    }
    class AddressDescriptor
    {
        private Address address { get; set; }
        public string StreetFieldName => "Street";
        public string GetStreet => this.address.Street;
        public string CityFieldName => "City";
        public string GetCity => this.address.City;

        public int NumOfFields = 2;

        public void FillValues(Address adress)
        {
            this.address = address;
        }
    }
    class CountryDescriptor
    {
        private Country country { get; set; }
        public string GetNameFieldName => "Name";
        public string GetName => this.country.Name;
        public string GetAreaCodeFieldName => "AreaCode";
        public string GetAreaCode => this.country.AreaCode.ToString();

        public int NumOfFileds = 2;
        public void FillValues(Country country)
        {
            this.country = country;
        }
    }
    class PhoneNumberDescriptor
    {
        private PhoneNumber phoneNumber { get; set; }
        public string GetCountryFieldName => "Country";
        public CountryDescriptor Country { get; set; }
        public string GetNumberFieldName => "Number";
        public string GetNmber => this.phoneNumber.Number.ToString();

        public int NumOfFilds = 2;
        public PhoneNumberDescriptor()
        {
            this.Country = new CountryDescriptor();
        }
        public void FillValues(PhoneNumber phone)
        {
            this.phoneNumber = phone;
            this.Country.FillValues(phone.Country);
        }
    }
}
