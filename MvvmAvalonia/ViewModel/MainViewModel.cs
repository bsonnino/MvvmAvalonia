using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomerLib;

namespace CustomerApp.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ICustomerRepository _customerRepository;
        private Func<Customer, bool> _filter = c => true;
        public IEnumerable<Customer> Customers => _customerRepository.Customers.Where(_filter);
        public Func<Customer, bool> Filter => _filter;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveCommand))]
        private Customer? _selectedCustomer;

        public MainViewModel(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ??
                                  throw new ArgumentNullException("customerRepository");
        }

        [RelayCommand]
        private void Add()
        {
            var customer = new Customer();
            _customerRepository.Add(customer);
            SelectedCustomer = customer;
            OnPropertyChanged(nameof(Customers));
        }

        [RelayCommand(CanExecute = "HasSelectedCustomer")]
        private void Remove()
        {
            if (SelectedCustomer != null)
            {
                _customerRepository.Remove(SelectedCustomer);
                SelectedCustomer = null;
                OnPropertyChanged(nameof(Customers));
            }
        }

        private bool HasSelectedCustomer() => SelectedCustomer != null;

        [RelayCommand]
        private void Save()
        {
            _customerRepository.Commit();
        }

        [RelayCommand]
        private void Search(string textToSearch)
        {
            if (!string.IsNullOrWhiteSpace(textToSearch))
                _filter = c => ((Customer)c).Country.ToLower().Contains(textToSearch.ToLower());
            else
                _filter = c => true;
            OnPropertyChanged(nameof(Customers));
        }
    }
}