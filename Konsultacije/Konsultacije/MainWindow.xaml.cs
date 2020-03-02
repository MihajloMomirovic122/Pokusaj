using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Konsultacije
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Konstruktori
        private ObservableCollection<CustomArtikal> potrebni { get; set; }
        private ObservableCollection<CustomOgr> maksimir { get; set; }
        
        private prdDataContext prd = new prdDataContext();
        #endregion Konstruktori

        #region Metode

        private void PokupiSlider()
        {
            maksimir = new ObservableCollection<CustomOgr>();
            maksimir.Add(new CustomOgr
            {
                MaxCena = prd.Artikals.Select(s=>s.Cena).Max(),
                MinCena = prd.Artikals.Select(s=>s.Cena).Min()
            });
            slider.Minimum = maksimir[0].MinCena;
            slider.Maximum = maksimir[0].MaxCena;

        }
  
        private void popuni() 
        {
            potrebni = new ObservableCollection<CustomArtikal>();
            foreach(var n in prd.Artikals.Select(s=> new { s.Naziv,s.Cena,s.SifraMagacina,s.SifraProizvoda,s.PDV})
                .Join(prd.zalihes, p=>p.SifraMagacina,k=>k.IDMagacin,(p,k) => new { p.SifraProizvoda,p.Naziv, p.Cena, p.PDV, k.Zaliha })) 
            {
                potrebni.Add(new CustomArtikal
                {
                    Naziv = n.Naziv,
                    Cena = n.Cena,
                    PDV = n.PDV,
                    SifraProizvoda = n.SifraProizvoda,
                    Zalihe = (int)n.Zaliha

                });
                
            }
                


         
            
            dataArtikli.ItemsSource = potrebni;
        }
       

        private void comboDobavljaci() 
        {
            var n = prd.Grupas;
            CmbDobavlj.ItemsSource = n;

        }
        #endregion Metode
        public MainWindow()
        {
            InitializeComponent();
            popuni();
            comboDobavljaci();
            PokupiSlider();
           
           
        }


        #region Dogadjaji
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int id, kol;
            if(Int32.TryParse(txtID.Text,out id) && Int32.TryParse(txtKol.Text,out kol) && kol>0) 
            {
                try 
                {
                    var pr = prd.Artikals.Where(s => s.SifraProizvoda == id).FirstOrDefault();
                    if (pr != null) 
                    {
                        zalihe za = prd.zalihes.Where(p => p.Zaliha >= kol && p.IDMagacin == pr.SifraMagacina).FirstOrDefault();
                        if (za != null) 
                        {
                            za.Zaliha -=kol;
                            prd.SubmitChanges();
                            popuni();
                        }
                        else { MessageBox.Show("Nema na stanju"); return; }
                    }
                    else { MessageBox.Show("Proizvod ne postoji");return; }

                    MessageBox.Show("Artikal:" + pr.Naziv + " prodano:" + kol);

                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else { MessageBox.Show("Pogresni parametri"); }
        }
       

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Window1 w1 = new Window1();
            w1.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Artikal a = new Artikal();
            a.Cena = 100;
            a.Naziv = "Fanta";
            a.SifraProizvoda = 7;
            a.IDGrupe = 3;
            a.PDV = 10;
            a.SifraMagacina = 2;
            try 
            {
                prd.Artikals.InsertOnSubmit(a);
                prd.SubmitChanges();
                popuni();
                MessageBox.Show("Uspeno dodavanje!");
            } 
            catch(Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        private void CmbDobavlj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show(((Grupa)CmbDobavlj.SelectedValue).Naziv);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((CustomArtikal)dataArtikli.SelectedValue).Naziv);
        }
        #endregion Dogadjaji

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach(Control n in gridCena.Children) 
            {
                if(n is Slider) 
                {
                    MessageBox.Show(((Slider)n).Value.ToString());
                }
            }
        }
    }
}
