using System;
using System.Collections.Generic;
using System.Text;

namespace UnreflectedSerializer
{
    class PersonDescriptor
    {
        public string GetThisName() => "Person";
        private Person person { get; set; }
        public string GetFirstNameFieldName() => "FirstName";
        public string GetFirstName() => this.person.FirstName;
        public string GetLastNameFieldName() => "LastName";
        public string GetLastName() => this.person.LastName;
        public string GetHomeAdressFieldName() => "HomeAddress";
        public AddressDescriptor HomeAddress { get; set; }
        public string GetWorkAdressFieldName() => "WorkAddress";
        public AddressDescriptor WorkAddress { get; set; }
        public string GetCitizenOfFieldName() => "CitizenOf";
        public CountryDescriptor CitizenOf { get; set; }
        public string GetMobilePhoneFieldName() => "MobilePhone";
        public PhoneNumberDescriptor MobilePhone { get; set; }

        public int NumOfFields = 6;
        public void GetDelegates(ref List<GetFieldName> getNames, ref List<GetValue> getValues, ref Queue<int> NumsOfFields)
        {  
            //Person itself
            NumsOfFields.Enqueue(this.NumOfFields);
            getNames.Add(new GetFieldName(GetThisName));
            //First Name
            NumsOfFields.Enqueue(1);
            getNames.Add(new GetFieldName(GetFirstNameFieldName));
            getValues.Add(new GetValue(GetFirstName));
            //Last Name
            NumsOfFields.Enqueue(1);
            getNames.Add(new GetFieldName(GetLastNameFieldName));
            getValues.Add(new GetValue(GetLastName));
            //Home Address
            getNames.Add(new GetFieldName(GetHomeAdressFieldName));
            this.HomeAddress.GetDelegates(ref getNames, ref getValues, ref NumsOfFields);
            //Work Address
            getNames.Add(new GetFieldName(GetWorkAdressFieldName));
            this.WorkAddress.GetDelegates(ref getNames, ref getValues, ref NumsOfFields);
            //Citizen Of
            getNames.Add(new GetFieldName(GetCitizenOfFieldName));
            this.CitizenOf.GetDelegates(ref getNames, ref getValues, ref NumsOfFields);
            //Mobile Phone
            getNames.Add(new Dele)
        }

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
        public string StreetFieldName()=> "Street";
        public string GetStreet() => this.address.Street;
        public string CityFieldName() => "City";
        public string GetCity() => this.address.City;

        public int NumOfFields = 2;
        public void GetDelegates(ref List<GetFieldName> getNames, ref List<GetValue> getValues, ref List<int> NumsOfFields)
        {
            //Adress itself
            NumsOfFields.Add(this.NumOfFields);
            //Street
            NumsOfFields.Add(1);
            getNames.Add(new GetFieldName(StreetFieldName));
            getValues.Add(new GetValue(GetStreet));
            //City
            NumsOfFields.Add(1);
            getNames.Add(new GetFieldName(CityFieldName));
            getValues.Add(new GetValue(GetCity));
        }

        public void FillValues(Address adress)
        {
            this.address = address;
        }
    }
    class CountryDescriptor
    {
        private Country country { get; set; }
        public string NameFieldName() => "Name";
        public string GetName() => this.country.Name;
        public string AreaCodeFieldName() => "AreaCode";
        public string GetAreaCode() => this.country.AreaCode.ToString();

        public int NumOfFields = 2;
        public void GetDelegates(ref List<GetFieldName> getNames, ref List<GetValue> getValues, ref List<int> NumsOfFields)
        {
            //Countrt itself
            NumsOfFields.Add(this.NumOfFields);
            //Name
            NumsOfFields.Add(1);
            getNames.Add(new GetFieldName(NameFieldName));
            getValues.Add(new GetValue(GetName));
            //Areas Code
            NumsOfFields.Add(1);
            getNames.Add(new GetFieldName(AreaCodeFieldName));
            getValues.Add(new GetValue(GetAreaCode));
        }

        public void FillValues(Country country)
        {
            this.country = country;
        }
    }
    class PhoneNumberDescriptor
    {
        private PhoneNumber phoneNumber { get; set; }
        public string GetCountryFieldName() => "Country";
        public CountryDescriptor Country { get; set; }
        public string GetNumberFieldName() => "Number";
        public string GetNumber() => this.phoneNumber.Number.ToString();

        public int NumOfFields = 2;
        public void GetDelegates(ref List<GetFieldName> getNames, ref List<GetValue> getValues, ref List<int> NumsOfFields)
        {
            //Phone number itself
            NumsOfFields.Add(this.NumOfFields);
            //Country
            getNames.Add(new GetFieldName(GetCountryFieldName));
            this.Country.GetDelegates(ref getNames, ref getValues, ref NumsOfFields);
            //Number 
            NumsOfFields.Add(1);
            getNames.Add(new GetFieldName(GetNumberFieldName));
            getValues.Add(new GetValue(GetNumber));
        }

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
