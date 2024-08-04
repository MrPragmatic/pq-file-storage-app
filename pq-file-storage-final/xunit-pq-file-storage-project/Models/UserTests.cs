using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Xunit;
using DataAccessClassLibrary.Models;

namespace xunit_pq_file_storage_project.Models
{
    public class UserTests
    {
        [Fact]
        public void UserPropertiesGettersAndSettersShouldWork()
        {
            var testUser = new User();
            var testId = 10;
            var testEmail = "test@example.ie";
            var testPassword = "NCIisProfessional11";

            testUser.Id = testId;
            testUser.Email = testEmail;
            testUser.Password = testPassword;

            // asserts
            Assert.Equal(testId, testUser.Id);
            Assert.Equal(testEmail, testUser.Email);
            Assert.Equal(testPassword, testUser.Password);
        }

        [Fact]
        public void RequiredUserEmailShouldThrowValidationError()
        {
            var testUser = new User
            {
                Email = ""
            };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(testUser);

            bool isValid = Validator.TryValidateObject(testUser, validationContext, validationResults, true);

            // asserts
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "The email field is required.");
        }

        [Fact]
        public void RequiredUserPasswordShouldThrowValidationError()
        {
            var testUser = new User
            {
                Password = ""
            };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(testUser);

            bool isValid = Validator.TryValidateObject(testUser, validationContext, validationResults, true);

            // asserts
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "The password field is required.");
        }

        [Fact]
        public void UserShouldBeValidWhenAllPropertiesAreSet()
        {
            var testUser = new User
            {
                Id = 11,
                Email = "validemail@gmail.com",
                Password = "ValidPassflwWod43%",
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(testUser);

            bool isValid = Validator.TryValidateObject(testUser, validationContext, validationResults, true);

            // assert
            Assert.True(isValid);
        }
    }
}
