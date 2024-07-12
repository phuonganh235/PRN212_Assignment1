using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;

namespace Assignment_1
{
    public partial class MainWindow : Window
    {
        private XDocument contactsDoc;
        private const string FilePath = "Contacts.xml";
        private ObservableCollection<Contact> contacts;

        public MainWindow()
        {
            InitializeComponent();
            contacts = new ObservableCollection<Contact>();
            lvPerson.ItemsSource = contacts;
        }

        private void loadData(object sender, RoutedEventArgs e)
        {
            contactsDoc = XDocument.Load(FilePath);
            foreach (var contact in contactsDoc.Root.Elements("Contact"))
            {
                contacts.Add(new Contact
                {
                    ID = contact.Attribute("Id").Value,
                    ContactName = contact.Element("ContactName").Value,
                    Company = contact.Element("Company").Value,
                    Phone = contact.Element("Phone").Value
                });
            }
        }

        private void btn_InsertClick(object sender, RoutedEventArgs e)
        {
            var id = txtID.Text;
            var name = txtContactName.Text;
            var company = txtCompany.Text;
            var phone = txtPhone.Text;

            if (IsValidContact(id, name, company, phone) && IsUnique(id, phone))
            {
                var newContact = new XElement("Contact",
                    new XAttribute("Id", id),
                    new XElement("ContactName", name),
                    new XElement("Company", company),
                    new XElement("Phone", phone));

                contactsDoc.Root.Add(newContact);
                contactsDoc.Save(FilePath);
                contacts.Add(new Contact { ID = id, ContactName = name, Company = company, Phone = phone });
                ClearForm();
            }
        }

        private void btn_UpdateClick(object sender, RoutedEventArgs e)
        {
            if (lvPerson.SelectedItem is Contact selectedContact)
            {
                var id = txtID.Text;
                var name = txtContactName.Text;
                var company = txtCompany.Text;
                var phone = txtPhone.Text;

                if (IsValidContact(id, name, company, phone) && IsUnique(id, phone, selectedContact))
                {
                    var contactElement = contactsDoc.Root.Elements("Contact")
                        .FirstOrDefault(c => c.Attribute("Id").Value == selectedContact.ID);

                    if (contactElement != null)
                    {
                        contactElement.SetElementValue("ContactName", name);
                        contactElement.SetElementValue("Company", company);
                        contactElement.SetElementValue("Phone", phone);
                        contactsDoc.Save(FilePath);

                        selectedContact.ContactName = name;
                        selectedContact.Company = company;
                        selectedContact.Phone = phone;
                        lvPerson.Items.Refresh();
                        ClearForm();
                    }
                }
            }
        }

        private void btn_DeleteClick(object sender, RoutedEventArgs e)
        {
            if (lvPerson.SelectedItem is Contact selectedContact)
            {
                var result = MessageBox.Show("Are you sure you want to delete this contact?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var contactElement = contactsDoc.Root.Elements("Contact")
                        .FirstOrDefault(c => c.Attribute("Id").Value == selectedContact.ID);

                    if (contactElement != null)
                    {
                        contactElement.Remove();
                        contactsDoc.Save(FilePath);
                        contacts.Remove(selectedContact);
                        ClearForm();
                    }
                }
            }
        }

        private void listView_Selected(object sender, RoutedEventArgs e)
        {
            if (lvPerson.SelectedItem is Contact selectedContact)
            {
                txtID.Text = selectedContact.ID;
                txtContactName.Text = selectedContact.ContactName;
                txtCompany.Text = selectedContact.Company;
                txtPhone.Text = selectedContact.Phone;
            }
        }

        private bool IsValidContact(string id, string name, string company, string phone)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("All fields must be filled.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var phoneRegex = new Regex(@"^\(?\d{3}\)?[- ]?\d{7}$");
            if (!phoneRegex.IsMatch(phone))
            {
                MessageBox.Show("Phone format is invalid. Use xxx-xxxxxxx or (xxx) xxxxxxx.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private bool IsUnique(string id, string phone, Contact excludeContact = null)
        {
            foreach (var contact in contacts)
            {
                if (contact == excludeContact) continue;

                if (contact.ID == id || contact.Phone == phone)
                {
                    return false;
                }
            }
            return true;
        }

        private void ClearForm()
        {
            txtID.Clear();
            txtContactName.Clear();
            txtCompany.Clear();
            txtPhone.Clear();
        }
    }

    public class Contact
    {
        public string ID { get; set; }
        public string ContactName { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
    }
}
