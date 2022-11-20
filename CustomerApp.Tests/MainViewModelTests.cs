using System;
using System.Collections.Generic;
using System.Linq;
using CustomerApp.ViewModel;
using CustomerLib;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomerApp.Tests
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void Constructor_NullRepository_ShouldThrow()
        {
            Action act = () => new MainViewModel(null);

            act.Should().Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("customerRepository"));
        }

        [TestMethod]
        public void Constructor_Customers_ShouldHaveValue()
        {
            var repository = A.Fake<ICustomerRepository>();
            var customers = new List<Customer>();
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm =  new MainViewModel(repository);

            vm.Customers.Should().BeEquivalentTo(customers);
        }

        [TestMethod]
        public void Constructor_SelectedCustomer_ShouldBeNull()
        {
            var repository = A.Fake<ICustomerRepository>();
            var vm = new MainViewModel(repository);

            vm.SelectedCustomer.Should().BeNull();
        }

        [TestMethod]
        public void AddCommand_ShouldAddInRepository()
        {
            var repository = A.Fake<ICustomerRepository>();
            var vm = new MainViewModel(repository);

            vm.AddCommand.Execute(null);
            A.CallTo(() => repository.Add(A<Customer>._)).MustHaveHappened();
        }

        [TestMethod]
        public void AddCommand_SelectedCustomer_ShouldNotBeNull()
        {
            var repository = A.Fake<ICustomerRepository>();
            var vm = new MainViewModel(repository);
            vm.AddCommand.Execute(null);
            vm.SelectedCustomer.Should().NotBeNull();
        }

        [TestMethod]
        public void AddCommand_ShouldNotifyCustomers()
        {
            var repository = A.Fake<ICustomerRepository>();
            var vm = new MainViewModel(repository);
            var wasNotified = false;
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Customers")
                    wasNotified = true;
            };
            vm.AddCommand.Execute(null);
            wasNotified.Should().BeTrue();
        }

        [TestMethod]
        public void RemoveCommand_SelectedCustomerNull_ShouldNotRemoveInRepository()
        {
            var repository = A.Fake<ICustomerRepository>();
            var vm = new MainViewModel(repository);
            vm.RemoveCommand.Execute(null);
            A.CallTo(() => repository.Remove(A<Customer>._)).MustNotHaveHappened();
        }

        [TestMethod]
        public void RemoveCommand_SelectedCustomerNotNull_ShouldRemoveInRepository()
        {
            var repository = A.Fake<ICustomerRepository>();
            var vm = new MainViewModel(repository);
            vm.SelectedCustomer = new Customer();
            vm.RemoveCommand.Execute(null);
            A.CallTo(() => repository.Remove(A<Customer>._)).MustHaveHappened();
        }

        [TestMethod]
        public void RemoveCommand_SelectedCustomer_ShouldBeNull()
        {
            var repository = A.Fake<ICustomerRepository>();
            var vm = new MainViewModel(repository);
            vm.SelectedCustomer = new Customer();
            vm.RemoveCommand.Execute(null);
            vm.SelectedCustomer.Should().BeNull();
        }

        [TestMethod]
        public void RemoveCommand_ShouldNotifyCustomers()
        {
            var repository = A.Fake<ICustomerRepository>();
            var vm = new MainViewModel(repository);
            vm.SelectedCustomer = new Customer(); 
            var wasNotified = false;
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Customers")
                    wasNotified = true;
            };
            vm.RemoveCommand.Execute(null);
            wasNotified.Should().BeTrue();
        }

        [TestMethod]
        public void SaveCommand_ShouldCommitInRepository()
        {
            var repository = A.Fake<ICustomerRepository>();
            var vm = new MainViewModel(repository);
            vm.SaveCommand.Execute(null);
            A.CallTo(() => repository.Commit()).MustHaveHappened();
        }

        [TestMethod]
        public void SearchCommand_WithText_ShouldSetFilter()
        {
            var customers = new List<Customer>
            {
                new Customer { Country = "a"},
                new Customer { Country = "text"},
                new Customer { Country = "b"},
                new Customer { Country = "texta"},
                new Customer { Country = "a"},
                new Customer { Country = "b"},
            };
            var repository = A.Fake<ICustomerRepository>();
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository);
            vm.SearchCommand.Execute("text");
            vm.Customers.Count().Should().Be(2);
        }

        [TestMethod]
        public void SearchCommand_WithoutText_ShouldSetFilter()
        {
            var customers = new List<Customer>
            {
                new Customer { Country = "a"},
                new Customer { Country = "text"},
                new Customer { Country = "b"},
                new Customer { Country = "texta"},
                new Customer { Country = "a"},
                new Customer { Country = "b"},
            };
            var repository = A.Fake<ICustomerRepository>();
            A.CallTo(() => repository.Customers).Returns(customers);
            var vm = new MainViewModel(repository);
            vm.SearchCommand.Execute("");
            vm.Customers.Count().Should().Be(6);
        }
    }
}
