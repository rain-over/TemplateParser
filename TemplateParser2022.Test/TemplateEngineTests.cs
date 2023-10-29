using Xunit;
using System;

namespace TemplateParser2022.Test
{
    public class TemplateEngineTests
    {
        /// <summary>
        /// Test engine substitution of a local datasource string property.
        /// </summary>
        [Fact]
        public void Test_basic_local_property_substitute()
        {
            var engine = new TemplateEngine();
            var dataSource = new
            {
                Name = "John"
            };
            var output = engine.Apply("Hello [Name]", dataSource);
            Assert.Equal("Hello John", output);
        }

        /// <summary>
        /// Test engine substitution of spanned ([object.property]) datasource string properties.
        /// </summary>
        [Fact]
        public void Test_spanned_local_property_substitute()
        {
            var engine = new TemplateEngine();
            var dataSource = new
            {
                Contact = new
                {
                    FirstName = "John",
                    LastName = "Smith"
                }
            };
            var output = engine.Apply("Hello [Contact.FirstName] [Contact.LastName]", dataSource);
            Assert.Equal("Hello John Smith", output);
        }

        /// <summary>
        /// Test engine substitution of scoped ([with]) datasource string properties.
        /// </summary>
        [Fact]
        public void Test_scoped_property_substitute()
        {
            var engine = new TemplateEngine();
            var dataSource = new
            {
                Contact = new
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Organisation = new
                    {
                        Name = "Acme Ltd",
                        City = "Auckland"
                    }
                }
            };
            var output = engine.Apply(@"[with Contact]Hello [FirstName] from [with Organisation][Name] in [City][/with][/with]", dataSource);
            Assert.Equal("Hello John from Acme Ltd in Auckland", output);
        }

        /// <summary>
        /// Test engine substitution of invalid datasource string properties.
        /// </summary>
        [Fact]
        public void Test_invalid_property_substitute()
        {
            var engine = new TemplateEngine();
            var dataSource = new
            {
                Name = "John"
            };
            var output = engine.Apply(@"Hello [InvalidProperty1] [InvalidProperty2] [Name]", dataSource);
            Assert.Equal("Hello   John", output);
        }

        /// <summary>
        /// Test engine substitution of a template without any tokens.
        /// </summary>
        [Fact]
        public void Test_no_property_substitute()
        {
            var engine = new TemplateEngine();
            var dataSource = new
            {
                Name = "John"
            };
            var output = engine.Apply("Hello there", dataSource);
            Assert.Equal("Hello there", output);
        }

        /// <summary>
        /// Test engine substitution of a template with formatted tokens.
        /// </summary>
        [Fact]
        public void Test_formatted_date_property_substitute()
        {
            var engine = new TemplateEngine();
            var dataSource = new {
                Today = new DateTimeOffset(1990, 12, 1, 0, 0, 0, 0, TimeSpan.Zero)
            };
            var output = engine.Apply("The current date is [Today \"d MMMM yyyy\"]", dataSource);
            Assert.Equal("The current date is 1 December 1990", output);
        }

        /// <summary>
        /// Test engine substitution of a template with tokens containing format arguments that should be ignored.
        /// </summary>
        [Fact]
        public void Test_unformattable_property_substitute_bonus()
        {
            var engine = new TemplateEngine();
            var dataSource = new {
                Name = "John"
            };
            var output = engine.Apply("Hello [Name \"d MMMM yyyy\"]", dataSource);
            Assert.Equal("Hello John", output);
        }

        /// <summary>
        /// Test engine substitution of nested scoped ([with]) datasource string properties.
        /// </summary>
        [Fact]
        public void Test_scoped_deeply_property_substitute()
        {
            var engine = new TemplateEngine();
            var dataSource = new
            {
                Contact = new
                {
                    Name = "John",
                    Organisation = new
                    {
                        Name = "Acme Ltd",
                        City = "Auckland",
                        Address = new
						{
                            Street = "123 Main Street"
						}
                    }
                }
            };
            var output = engine.Apply(@"[with Contact]Hello [Name] from [with Organisation][Name] in [City] at [with Address][Street].[/with][/with] [Name], We hope to see you again soon.[/with]", dataSource);
            Assert.Equal("Hello John from Acme Ltd in Auckland at 123 Main Street. John, We hope to see you again soon.", output);
        }

        /// <summary>
        /// Test engine substitution of nested scoped ([with]) datasource string properties.
        /// </summary>
        [Fact]
        public void Test_multiple_scoped_properties_substitute()
        {
            var engine = new TemplateEngine();
            var dataSource = new
            {
                Contact = new
                {
                    FirstName = "John",
                    LastName = "Smith"
                },
                Organisation = new
                {
                    Name = "Acme Ltd",
                    City = "Auckland"
                }
            };
            var output = engine.Apply(@"[with Contact]Hello [FirstName]. [/with][with Organisation]Welcome to [Name][/with]", dataSource);
            Assert.Equal("Hello John. Welcome to Acme Ltd", output);
        }

        /// <summary>
        /// Test engine substitution of spanned ([object.property]) nested within a scoped ([with]) datasource string properties.
        /// </summary>
        [Fact]
        public void Test_scoped_and_spanned_property_substitute()
        {
            var engine = new TemplateEngine();
            var dataSource = new
            {
                Contact = new
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Organisation = new
                    {
                        Name = "Acme Ltd",
                        City = "Auckland"
                    }
                }
            };
            var output = engine.Apply(@"[with Contact]Hello [FirstName] from [Organisation.Name] in [Organisation.City][/with]", dataSource);
            Assert.Equal("Hello John from Acme Ltd in Auckland", output);
        }

        /// <summary>
        /// Test engine substitution of spanned ([object.object.property]) datasource string properties.
        /// </summary>
        [Fact]
        public void Test_super_spanned_property_substitute()
        {
            var engine = new TemplateEngine();
            var dataSource = new
            {
                Contact = new
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Organisation = new
                    {
                        Name = "Acme Ltd",
                        City = "Auckland"
                    }
                }
            };
            var output = engine.Apply(@"Hello [Contact.FirstName] from [Contact.Organisation.Name] in [Contact.Organisation.City]", dataSource);
            Assert.Equal("Hello John from Acme Ltd in Auckland", output);
        }

        /// <summary>
        /// Test engine substitution of spanned ([object.object.property]) containing format arguments datasource string properties.
        /// </summary>
        [Fact]
        public void Test_super_spanned_property_substitute_with_format()
        {
            var engine = new TemplateEngine();
            var dataSource = new
            {
                Contact = new
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Organisation = new
                    {
                        Name = "Acme Ltd",
                        City = "Auckland",
                        When = new DateTimeOffset(1990, 12, 1, 0, 0, 0, 0, TimeSpan.Zero)
                    }
                }
            };
            var output = engine.Apply("Hello [Contact.FirstName] from [Contact.Organisation.Name] in [Contact.Organisation.City] at [Contact.Organisation.When \"d MMMM yyyy\"]", dataSource);
            Assert.Equal("Hello John from Acme Ltd in Auckland at 1 December 1990", output);
        }       
    }
}
