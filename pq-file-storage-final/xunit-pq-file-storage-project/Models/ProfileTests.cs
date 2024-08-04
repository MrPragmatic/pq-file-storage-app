using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.ComponentModel.DataAnnotations;
using DataAccessClassLibrary.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Runtime.CompilerServices;

namespace xunit_pq_file_storage_project.Models
{
    public class ProfileTests
    {
        [Fact]
        public void ProfilePropertesGettersAndSettersShouldWork()
        {
            var testProfile = new Profile();
            int testId = 12;
            string testEmail = "test@example.ie";
            DateTime testEmailConfirmedAt = DateTime.Now;

            testProfile.Id = testId;
            testProfile.Email = testEmail;
            testProfile.EmailConfirmedAt = testEmailConfirmedAt;

            // asserts
            Assert.Equal(testId, testProfile.Id);
            Assert.Equal(testEmail, testProfile.Email);
            Assert.Equal(testEmailConfirmedAt, testProfile.EmailConfirmedAt);
        }

        [Fact]
        public void ProfileEmailShouldBeInValidFormat()
        {
            var testProfile = new Profile
            {
                Email = "wrong-email"
            };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(testProfile);

            bool isValid = Validator.TryValidateObject(testProfile, validationContext, validationResults, true);

            // assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "The email you entered is not a valid e-mail address. Please write a valid email to register.");
        }

        [Fact]
        public void RequiredProfileEmailShouldThrowError()
        {
            var testProfile = new Profile {
                Email = ""
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(testProfile);

            bool isValid = Validator.TryValidateObject(testProfile, validationContext, validationResults, true);

            // assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "The email field is required.");
        }

        [Fact]
        public void ProfileEmailShoudAcceptValidEmailAddress()
        {
            var testProfile = new Profile
            {
                Email = "test@example.ie"
            };
            var validationContext = new ValidationContext(testProfile)
            {
                MemberName = "Email"
            };

            // assert
            Validator.ValidateProperty(testProfile.Email, validationContext);
        }
        [Fact]
        public void ProfileShouldAcceptValidEmailConfirmedAtValue()
        {
            var testProfile = new Profile
            {
                EmailConfirmedAt = DateTime.Now
            };

            // assert
            Assert.NotNull(testProfile.EmailConfirmedAt);
        }
    }
}
