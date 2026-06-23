using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public static void Main()
    {
        TestNegozioOnline.EseguiTuttiITest();
    }
}

// --- CLASSI DI LOGICA ---
public class ApplicazioneNegozio
{
    public CatalogoProdotti catalogoProdotti = new CatalogoProdotti();
    public CarrelloUtente carrelloUtente = new CarrelloUtente();
    public StoricoAcquisti storicoAcquisti = new StoricoAcquisti();
    public ServizioNegozio servizioNegozio;
    public ApplicazioneNegozio() { servizioNegozio = new ServizioNegozio(catalogoProdotti, carrelloUtente, storicoAcquisti); }
}

public class Prodotto {
    public string CodiceProdotto { get; private set; } public string Nome { get; private set; } public decimal Prezzo { get; private set; } public int QuantitaDisponibile { get; private set; } public int QuantitaIniziale { get; private set; }
    public Prodotto(string c, string n, decimal p, int q) { CodiceProdotto = c; Nome = n; Prezzo = p; QuantitaDisponibile = q; QuantitaIniziale = q; }
    public void CambiaPrezzo(decimal np) { if (np <= 0) throw new ArgumentException(); Prezzo = np; }
    public void CambiaQuantita(int v) { if (QuantitaDisponibile + v < 0) throw new InvalidOperationException(); QuantitaDisponibile += v; }
    public int CalcolaQuantitaVenduta() => QuantitaIniziale - QuantitaDisponibile;
}

public class Utente { public string Nome { get; private set; } public Utente(string n) => Nome = n; }

public class ElementoCarrello {
    public Prodotto ProdottoSelezionato { get; private set; } public int QuantitaScelta { get; private set; } public decimal PrezzoUnitario { get; private set; }
    public ElementoCarrello(Prodotto p, int q) { ProdottoSelezionato = p; QuantitaScelta = q; PrezzoUnitario = p.Prezzo; }
    public decimal CalcolaTotaleParziale() => PrezzoUnitario * QuantitaScelta;
    public void CambiaQuantitaScelta(int q) { QuantitaScelta = q; }
}

public class Acquisto {
    public string NomeUtente { get; private set; } public List<ElementoAcquistato> ProdottiAcquistati { get; private set; } public decimal TotaleOrdine { get; private set; } public DateTime DataAcquisto { get; private set; }
    public Acquisto(Utente u, List<ElementoAcquistato> p) { NomeUtente = u.Nome; ProdottiAcquistati = p; DataAcquisto = DateTime.Now; TotaleOrdine = p.Sum(x => x.TotaleParziale); }
}

public class ElementoAcquistato {
    public string CodiceProdotto, NomeProdotto; public int QuantitaAcquistata; public decimal PrezzoUnitario, TotaleParziale;
    public ElementoAcquistato(string c, string n, int q, decimal p) { CodiceProdotto = c; NomeProdotto = n; QuantitaAcquistata = q; PrezzoUnitario = p; TotaleParziale = q * p; }
}

public class ReportProdotto {
    public string CodiceProdotto, NomeProdotto; public int QuantitaIniziale, QuantitaVenduta, QuantitaDisponibile;
    public ReportProdotto(string c, string n, int qi, int qv, int qd) { CodiceProdotto = c; NomeProdotto = n; QuantitaIniziale = qi; QuantitaVenduta = qv; QuantitaDisponibile = qd; }
}

public class CatalogoProdotti {
    private List<Prodotto> prodotti = new List<Prodotto>();
    public void AggiungiProdotto(Prodotto p) { if (prodotti.Any(x => x.CodiceProdotto.Equals(p.CodiceProdotto, StringComparison.OrdinalIgnoreCase))) throw new InvalidOperationException("Codice duplicato"); prodotti.Add(p); }
    public bool EliminaProdotto(string c) => prodotti.RemoveAll(x => x.CodiceProdotto.Equals(c, StringComparison.OrdinalIgnoreCase)) > 0;
    public Prodotto? CercaProdottoPerCodice(string c) => prodotti.FirstOrDefault(x => x.CodiceProdotto.Equals(c, StringComparison.OrdinalIgnoreCase));
    public List<Prodotto> OttieniTuttiIProdotti() => new List<Prodotto>(prodotti);
    public bool ModificaPrezzoProdotto(string c, decimal p) { var pr = CercaProdottoPerCodice(c); if(pr == null) return false; pr.CambiaPrezzo(p); return true; }
    public bool ModificaQuantitaProdotto(string c, int v) { var pr = CercaProdottoPerCodice(c); if(pr == null) return false; pr.CambiaQuantita(v); return true; }
}

public class CarrelloUtente {
    private List<ElementoCarrello> elementi = new List<ElementoCarrello>();
    public bool AggiungiAlCarrello(Prodotto p, int q) { 
        if (q <= 0) return false;
        var esistente = elementi.FirstOrDefault(e => e.ProdottoSelezionato.CodiceProdotto.Equals(p.CodiceProdotto, StringComparison.OrdinalIgnoreCase));
        int quantitaAttuale = esistente != null ? esistente.QuantitaScelta : 0;
        if (quantitaAttuale + q > p.QuantitaDisponibile) return false;
        if (esistente != null) esistente.CambiaQuantitaScelta(quantitaAttuale + q);
        else elementi.Add(new ElementoCarrello(p, q));
        return true; 
    }
    public bool ModificaQuantitaNelCarrello(string c, int q) { 
        var el = elementi.FirstOrDefault(e => e.ProdottoSelezionato.CodiceProdotto.Equals(c, StringComparison.OrdinalIgnoreCase));
        if(el == null || q <= 0 || q > el.ProdottoSelezionato.QuantitaDisponibile) return false;
        el.CambiaQuantitaScelta(q); return true; 
    }
    public bool RimuoviDalCarrello(string c) => elementi.RemoveAll(e => e.ProdottoSelezionato.CodiceProdotto.Equals(c, StringComparison.OrdinalIgnoreCase)) > 0;
    public decimal CalcolaTotale() => elementi.Sum(x => x.CalcolaTotaleParziale());
    public List<ElementoCarrello> OttieniElementi() => new List<ElementoCarrello>(elementi);
    public void SvuotaCarrello() => elementi.Clear();
}

public class StoricoAcquisti {
    private List<Acquisto> acquisti = new List<Acquisto>();
    public void RegistraAcquisto(Acquisto a) => acquisti.Add(a);
    public List<Acquisto> OttieniTuttiGliAcquisti() => new List<Acquisto>(acquisti);
    public List<Acquisto> OttieniAcquistiPerUtente(string n) => acquisti.Where(a => a.NomeUtente.Equals(n, StringComparison.OrdinalIgnoreCase)).ToList();
}

public class ServizioNegozio {
    private CatalogoProdotti cat; private CarrelloUtente car; private StoricoAcquisti sto;
    public ServizioNegozio(CatalogoProdotti c, CarrelloUtente cr, StoricoAcquisti st) { cat = c; car = cr; sto = st; }
    public bool AggiungiProdottoAlCarrello(string c, int q) { var p = cat.CercaProdottoPerCodice(c); if(p == null) return false; return car.AggiungiAlCarrello(p, q); }
    public Acquisto ConfermaAcquisto(Utente u) {
        var el = car.OttieniElementi();
        if (el.Count == 0) throw new InvalidOperationException("Carrello vuoto");
        var lista = el.Select(e => new ElementoAcquistato(e.ProdottoSelezionato.CodiceProdotto, e.ProdottoSelezionato.Nome, e.QuantitaScelta, e.PrezzoUnitario)).ToList();
        foreach(var e in el) e.ProdottoSelezionato.CambiaQuantita(-e.QuantitaScelta);
        var nuovoAcquisto = new Acquisto(u, lista);
        sto.RegistraAcquisto(nuovoAcquisto);
        car.SvuotaCarrello();
        return nuovoAcquisto;
    }
    public List<ReportProdotto> CreaReportProdotti() => cat.OttieniTuttiIProdotti().Select(p => new ReportProdotto(p.CodiceProdotto, p.Nome, p.QuantitaIniziale, p.CalcolaQuantitaVenduta(), p.QuantitaDisponibile)).ToList();
    public void StampaAcquisto(Acquisto a) { }
    public void StampaReportProdotti() { }
}