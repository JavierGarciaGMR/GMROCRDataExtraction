using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Entities
{

    public class Account : SignUp
    {
        public string IsAMCNPortal { get; set; }
        public string MembershipNumber { get; set; }
        public string AssociatedMembershipNumber { get; set; }
        public string TestFlags { get; set; }
    }

    public class Address
    {
        public StandarAddress HomeAddress { get; set; }
        public StandarAddress MailingAddress { get; set; }
        public StandarAddress BillingAddress { get; set; }
    }

    public class ContactInfo
    {
        public string PrimaryContactNumber { get; set; }
        public string AlternateNumber { get; set; }
        public string MobileNumber { get; set; }
        public string OtherPhone1 { get; set; }
        public string Email { get; set; }
    }

    public class Member
    {
        public int MemberId { get; set; }
        public string SlxMemberId { get; set; }
        public string Action { get; set; }
        public string DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }

        [DefaultValue("Other")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string ParticipantType { get; set; }
        public string Status { get; set; }
    }

    public class MembershipBundled
    {
        public string EffectiveDate { get; set; }
        public List<PlanInfo> planInfo { get; set; }
        public string StoreCategoryId { get; set; }
        public List<Member> Members { get; set; }
        public string Initials { get; set; }
        public string AuthorizationInitials { get; set; }
    }

    public class PlanInfo
    {
        public string OwnerDescription { get; set; }
        public string Term { get; set; }
        public string PlanName { get; set; }
        public string PlanType { get; set; }
        public decimal PlanAmount { get; set; }
        public string CouponCode { get; set; }

        [JsonIgnore]
        public string SeccodeId { get; set; }

        [JsonIgnore]
        public string OriginalOwnerDescription { get; set; }

        public bool IsActive { get; set; }
        public string ExpirationDate { get; set; }
        public bool IsExpired { get; set; }
    }

    public class SignUp : SignUpRequest
    {
        public bool IsMemberPresent { get; set; }
        public string AcceptedStateDisclaimer { get; set; }
    }

    public class SignUpRequest
    {
        public string VendorKey { get; set; }
        public string AccountIdToCreate { get; set; }
        public string HasEvent { get; set; }
        public string GeoLocationState { get; set; }
        public ContactInfo contactInfo { get; set; }
        public MembershipBundled membership { get; set; }
        public Address address { get; set; }
        public List<SLXConstants> slxConstantInfo { get; set; }
    }

    public class SLXConstants
    {
        public string SecCodeId { get; set; }
        public string PlanCodeID { get; set; }
        public string MbrsvRep { get; set; }
        public string GetCodeId { get; set; }
        public string TrackCodeId { get; set; }
        public string TpaTrackCodeId { get; set; }
        public string ContactMethod { get; set; }
        public string HowDidYouHearAboutUs { get; set; }
        public string Status { get; set; }
        public string AccountType { get; set; }
        public string ImportSource { get; set; }
        public string MembershipProgram { get; set; }
        public string MembershipSubProgram { get; set; }
        public string OwnerDescription { get; set; }
    }

    public class StandarAddress
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string StateProvinceAbbrevation { get; set; }

        [DefaultValue("US")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string CountryCode { get; set; }
    }

}



