using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VisaNet.Utilities.Helpers.UnitTestProject
{
    [TestClass]
    public class EmailValidatorTest
    {
        private EmailValidator _validator;

        [TestInitialize]
        public void InitializeTest()
        {
            _validator = new EmailValidator();
        }

        [TestMethod]
        public void ShouldAceptValidMail()
        {
            var res = _validator.IsValid("fernandezllambi@outlook.com");            
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ShouldAceptUpperCase()
        {
            var res = _validator.IsValid("FernandezLlambi@outlook.com");
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ShouldAceptDot()
        {
            var res = _validator.IsValid("Fernandez.Llambi@outlook.com");
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ShouldAceptDash()
        {
            var res = _validator.IsValid("Fernandez-Llambi@outlook.com");
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ShouldAceptUnderscore()
        {
            var res = _validator.IsValid("Fernandez_Llambi@outlook.com");
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ShouldRejectAccentMark()
        {
            var res = _validator.IsValid("fernandezllambí@outlook.com");
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ShouldRejectDoubleAccentMark()
        {
            var res = _validator.IsValid("fernándezllambí@outlook.com");
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ShouldRejectAccentMarkUpperCase()
        {
            var res = _validator.IsValid("fernandezllambÍ@outlook.com");
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ShouldRejectDoubleAccentMarkUpperCase()
        {
            var res = _validator.IsValid("fernÁndezllambÍ@outlook.com");
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ShouldRejectSpecialCharacter()
        {
            var res = _validator.IsValid("fernandezllambï@outlook.com");
            Assert.IsFalse(res);
        }
        
        [TestMethod]
        public void ShouldRejectSpace()
        {
            var res = _validator.IsValid("fernandez llambi@outlook.com");
            Assert.IsFalse(res);
        }
        
        [TestMethod]
        public void ShouldRejectSpaces()
        {
            var res = _validator.IsValid("fer nandez llambi@outlook.com");
            Assert.IsFalse(res);
        }
        
        [TestMethod]
        public void ShouldAceptComplexDomain()
        {
            var res = _validator.IsValid("fernandezllambi@outlook.com.uy");
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ShouldRejectNumeral()
        {
            var res = _validator.IsValid("fernandez#llambi@outlook.com");
            Assert.IsFalse(res);
        } 
        
        [TestMethod]
        public void ShouldRejectDollar()
        {
            var res = _validator.IsValid("fernandez$llambi@outlook.com");
            Assert.IsFalse(res);
        } 
        
        [TestMethod]
        public void ShouldRejectPercent()
        {
            var res = _validator.IsValid("fernandez%llambi@outlook.com");
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ShouldRejectParentesis()
        {
            var res = _validator.IsValid("fernandez(llambi)@outlook.com");
            Assert.IsFalse(res);
        }
        
        [TestMethod]
        public void ShouldRejectComma()
        {
            var res = _validator.IsValid("fernandez,llambi@outlook.com");
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ShouldAceptPlus()
        {
            var res = _validator.IsValid("fernandez+llambi@outlook.com");
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ShouldAceptNull()
        {
            var res = _validator.IsValid(null);
            Assert.IsTrue(res);
        }
        
        [TestMethod]
        public void ShouldAceptEmptyString()
        {
            var res = _validator.IsValid("");
            Assert.IsTrue(res);
        }
        
        [TestMethod]
        public void ShouldRejectWhiteSpaces()
        {
            var res = _validator.IsValid(" ");
            Assert.IsFalse(res);
        }
    }
}
