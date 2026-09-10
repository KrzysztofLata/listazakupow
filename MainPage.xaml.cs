using System.Collections.ObjectModel;
using ZXing.Net.Maui;

namespace MauiApp2
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Item> Products { get; set; }

        public MainPage()
        {
            InitializeComponent();
            Products = new ObservableCollection<Item>();
            BindingContext = this;
        }
        protected override void OnAppearing() // nadpisanie domyślnej metody OnAppearing która jest wywoływana gdy strona staje się widoczna
        {
            base.OnAppearing();

            // Konfiguracja formatów
            cameraView.Options = new BarcodeReaderOptions
            {
                Formats = BarcodeFormats.OneDimensional | BarcodeFormats.TwoDimensional,
                AutoRotate = true,
                Multiple = true,
                TryHarder = true
            };
        }
        private void CameraView_BarcodeDetected(object sender, BarcodeDetectionEventArgs e)
        {
            var result = e?.Results?.FirstOrDefault(); // Jeśli e nie jest null pobierz wynik(kolekcje wyników) i weź pierwszy element(FirstOrDefault)
            if (result is null)
                return;
            // Wywołanie na wątku UI i przypisanie do labela, kamera działa na wątku tła i pobranie wyniku z kamery nie może być przypisane do labela, ponieważ label działa na wątku UI,
            // wątek tła może nadal działać i wykrywać kolejne kody
            // Podstawowe wątki: MainThead - wątek UI, Background Thread - wątek tła, w którym działa kamera, 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                String itemname = result.Value;
                String quantity = setquantity.Text;
                if (quantity == null || quantity == "")
                {
                    quantity = "1";
                }
                if (string.IsNullOrEmpty(itemname))
                    return;
                var existing = Products.FirstOrDefault(p => p.Name.Equals(itemname, StringComparison.OrdinalIgnoreCase));

                if (existing != null)
                {
                    Products.Remove(existing);
                    Products.Add(existing);
                }
                else
                {
                    Products.Add(new Item { Name = itemname, Quantity = Int32.Parse(quantity) });
                }

                collectionview.ItemsSource = Products;
            });
        }
        private void AddBtn_Clicked(object sender, EventArgs e)
        {
            string itemname = additem.Text;
            String quantity = setquantity.Text;
            if (quantity==null || quantity=="")
            {
                quantity = "1";
            }
            if (string.IsNullOrEmpty(itemname))
                return;
            additem.Text = "";
            setquantity.Text = "";
            var existing = Products.FirstOrDefault(p => p.Name.Equals(itemname, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.Quantity++;
                Products.Remove(existing);
                Products.Add(existing);
            }
            else
            {
                Products.Add(new Item { Name = itemname, Quantity = Int32.Parse(quantity) });
            }

            collectionview.ItemsSource = Products;
        }

        private void DelBtn_Clicked(object sender, EventArgs e)
        {
            var selected = collectionview.SelectedItem;
            Products.Remove((Item)selected);
        }
    }
}
