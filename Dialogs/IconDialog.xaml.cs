using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MDEditor.Dialogs;

public partial class IconDialog : Window
{
    public string? SelectedCode { get; private set; }
    public bool IsEmoji { get; private set; }

    private readonly List<IconEntry> _allEmojis;
    private readonly List<IconEntry> _allIcons;
    private bool _showingEmoji = true;

    public IconDialog()
    {
        InitializeComponent();

        _allEmojis = GetEmojiList();
        _allIcons = GetFontAwesomeList();

        Owner = Application.Current.MainWindow;
        ShowEmojiView();
        SearchBox.Focus();
    }

    private void ShowEmojiView()
    {
        _showingEmoji = true;
        TabEmoji.FontWeight = FontWeights.Bold;
        TabFA.FontWeight = FontWeights.Normal;
        PopulateGrid(_allEmojis);
    }

    private void ShowFAView()
    {
        _showingEmoji = false;
        TabFA.FontWeight = FontWeights.Bold;
        TabEmoji.FontWeight = FontWeights.Normal;
        PopulateGrid(_allIcons);
    }

    private void PopulateGrid(List<IconEntry> items)
    {
        IconGrid.Children.Clear();
        var searchText = SearchBox.Text.Trim().ToLowerInvariant();

        var filtered = string.IsNullOrEmpty(searchText)
            ? items
            : items.Where(i => i.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            || i.Keywords.Any(k => k.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
                   .ToList();

        foreach (var item in filtered)
        {
            var btn = new Button
            {
                Style = (Style)FindResource("IconButton"),
                Content = item.Display,
                Tag = item.Code,
                FontFamily = item.IsFA ? new FontFamily("Segoe UI") : new FontFamily("Segoe UI Emoji, Segoe UI"),
                FontSize = item.IsFA ? 10 : 18,
                Width = item.IsFA ? 90 : 42,
                Height = item.IsFA ? 46 : 38
            };
            if (item.IsFA)
            {
                // Show icon name as text
                var sp = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
                sp.Children.Add(new TextBlock
                {
                    Text = "🎨",
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                sp.Children.Add(new TextBlock
                {
                    Text = item.Code.Replace(":fa[", "").Replace("]", ""),
                    FontSize = 9,
                    Foreground = FindResource("ForegroundBrush") as System.Windows.Media.Brush,
                    Opacity = 0.7,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new System.Windows.Thickness(0, 2, 0, 0)
                });
                btn.Content = sp;
            }
            btn.Click += IconButton_Click;
            IconGrid.Children.Add(btn);
        }
    }

    private void IconButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string code)
        {
            SelectedCode = code;
            IsEmoji = _showingEmoji;

            // Update preview
            IconPreview.Text = _showingEmoji ? code : $"<i class='fa-solid fa-{code.Replace(":fa[","").Replace("]","")}'></i>";
            CodePreview.Text = code;
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        Placeholder.Visibility = string.IsNullOrEmpty(SearchBox.Text)
            ? Visibility.Visible : Visibility.Collapsed;
        PopulateGrid(_showingEmoji ? _allEmojis : _allIcons);
    }

    private void TabEmoji_Click(object sender, RoutedEventArgs e) => ShowEmojiView();
    private void TabFA_Click(object sender, RoutedEventArgs e) => ShowFAView();

    private void Insert_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedCode))
        {
            // Take first visible icon
            if (IconGrid.Children.Count > 0 && IconGrid.Children[0] is Button firstBtn)
            {
                SelectedCode = firstBtn.Tag as string ?? "";
                IsEmoji = _showingEmoji;
            }
        }

        if (!string.IsNullOrEmpty(SelectedCode))
        {
            DialogResult = true;
            Close();
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    // =============================================
    //  EMOJI CATALOG
    // =============================================
    private static List<IconEntry> GetEmojiList()
    {
        return new List<IconEntry>
        {
            // Faccine
            new("😀", "faccina sorridente", "sorriso", "felice", "faccia", "smile"),
            new("😂", "faccina che ride", "risata", "lol", "divertente"),
            new("🤣", "rotolarsi dal ridere", "risata", "lol"),
            new("😍", "faccina con cuori", "amore", "cuore", "innamorato"),
            new("😎", "faccina con occhiali", "cool", "sole", "occhiali"),
            new("🤔", "faccina pensierosa", "pensare", "dubbio", "hmm"),
            new("😢", "faccina triste", "triste", "pianto", "lacrima"),
            new("😡", "faccina arrabbiata", "arrabbiato", "rabbia"),
            new("🥳", "faccina festeggiante", "festa", "compleanno", "party"),
            new("🤯", "mente esplosa", "mind blown", "esplosione", "sorpresa"),
            new("🤩", "occhi a stella", "star struck", "wow"),
            new("😴", "faccina addormentata", "sonno", "dormire", "zzz"),
            new("🤗", "faccina abbraccio", "abbraccio", "hug"),
            new("😱", "faccina spaventata", "paura", "urlo", "scream"),
            new("🤐", "bocca chiusa", "zip", "segreto", "shh"),
            new("🥺", "occhi teneri", "pleading", "cute", "tenero"),
            new("😏", "faccina sorniona", "smirk", "sornione"),
            new("🙄", "occhi al cielo", "roll eyes", "annoiato"),
            new("😬", "faccina a denti stretti", "grimace", "imbarazzato"),
            new("🤫", "zittire", "shush", "silenzio"),
            new("🤤", "sbavando", "drooling", "fame"),
            new("😪", "assonnato", "sleepy", "sonno", "stanco"),

            // Gesti / Mani
            new("👍", "pollice su", "ok", "like", "buono"),
            new("👎", "pollice giù", "no", "dislike", "cattivo"),
            new("👏", "applauso", "batti", "clap", "bravo"),
            new("🙌", "mani alzate", "celebration", "evviva", "yay"),
            new("🤝", "stretta di mano", "accordo", "handshake", "deal"),
            new("💪", "bicipite", "forza", "muscolo", "strong"),
            new("✌️", "pace", "vittoria", "peace", "victory"),
            new("🤞", "dita incrociate", "fortuna", "crossed fingers"),
            new("👌", "ok", "perfetto", "okay"),
            new("✍️", "scrivere", "penna", "scrittura"),
            new("🙏", "preghiera", "grazie", "please", "thanks"),
            new("🤲", "palmi aperti", "palms up", "preghiera"),

            // Oggetti
            new("⭐", "stella", "star", "preferito", "importante"),
            new("🔥", "fuoco", "hot", "fiamma", "popolare", "trending"),
            new("💡", "lampadina", "idea", "lightbulb", "pensiero"),
            new("❤️", "cuore rosso", "love", "amore"),
            new("💙", "cuore blu", "love blue"),
            new("💚", "cuore verde", "love green"),
            new("💛", "cuore giallo", "love yellow"),
            new("🎉", "festa", "party", "celebrazione", "congratulations"),
            new("🎯", "bersaglio", "target", "obiettivo", "centrato"),
            new("💻", "computer", "pc", "laptop", "tech"),
            new("📱", "smartphone", "telefono", "mobile", "phone"),
            new("🖥️", "desktop", "monitor", "schermo"),
            new("📝", "appunti", "memo", "note", "scrivere"),
            new("📌", "puntina", "pin", "fissare"),
            new("📎", "graffetta", "clip", "allegato"),
            new("✂️", "forbici", "taglia", "cut"),
            new("🔍", "lente sinistra", "cerca", "search"),
            new("🔎", "lente destra", "cerca", "search"),
            new("🔑", "chiave", "password", "key", "accesso"),
            new("🔒", "lucchetto chiuso", "lock", "sicurezza", "privato"),
            new("🔓", "lucchetto aperto", "unlock", "pubblico"),
            new("🏠", "casa", "home", "inizio"),
            new("📚", "libri", "books", "studio", "documentazione"),
            new("📖", "libro aperto", "open book", "leggere"),
            new("🛠️", "attrezzi", "tools", "strumenti", "configurazione"),
            new("⚙️", "ingranaggio", "gear", "settings", "opzioni"),
            new("🧰", "cassetta attrezzi", "toolbox"),
            new("🔧", "chiave inglese", "wrench", "riparare"),
            new("🔨", "martello", "hammer", "build"),
            new("💾", "floppy disk", "save", "salva"),
            new("📁", "cartella", "folder", "directory"),
            new("📂", "cartella aperta", "open folder"),
            new("🗂️", "cartelle", "folders"),
            new("📊", "grafico a barre", "chart", "statistiche", "analytics"),
            new("📈", "grafico in salita", "trending up", "crescita"),
            new("📉", "grafico in discesa", "trending down"),
            new("🏆", "trofeo", "trophy", "vincitore", "successo"),
            new("🥇", "medaglia d'oro", "gold", "primo"),
            new("🥈", "medaglia d'argento", "silver", "secondo"),
            new("🥉", "medaglia di bronzo", "bronze", "terzo"),
            new("🎓", "cappello laurea", "graduation", "studio"),
            new("🏷️", "etichetta", "label", "tag"),
            new("📦", "pacco", "package", "box"),
            new("📫", "cassetta posta", "mailbox"),
            new("📬", "cassetta posta piena", "mail"),
            new("📧", "email", "mail", "e-mail"),
            new("🔔", "campana", "bell", "notifica", "alert"),
            new("🔕", "campana muta", "silenzioso", "mute"),
            new("💬", "fumetto", "commento", "chat"),
            new("🗨️", "fumetto sinistro", "chat left"),
            new("📢", "megafono", "announcement", "annuncio"),
            new("💰", "borsa soldi", "money", "soldi", "prezzo"),
            new("💳", "carta credito", "credit card", "pagamento"),
            new("🛒", "carrello", "cart", "shopping", "acquista"),
            new("🎁", "regalo", "gift", "presente"),
            new("🧩", "puzzle", "componente", "modulo"),
            new("🧪", "provetta", "test", "esperimento"),
            new("🧬", "dna", "science", "biologia"),
            new("🔬", "microscopio", "science", "ricerca"),
            new("🔭", "telescopio", "osservare"),
            new("🌍", "mondo america", "globe", "mondo"),
            new("🌎", "mondo americhe", "globe"),
            new("🌏", "mondo asia", "globe"),
            new("🗺️", "mappa", "map", "world map"),
            new("🧭", "bussola", "compass", "direzione"),

            // Simboli / Segni
            new("✅", "spunta verde", "check", "done", "completato", "ok"),
            new("❌", "croce rossa", "cross", "error", "no", "cancellato"),
            new("⚠️", "attenzione", "warning", "alert", "importante"),
            new("🚫", "divieto", "prohibited", "vietato", "no"),
            new("➕", "più", "plus", "aggiungi", "add"),
            new("➖", "meno", "minus", "rimuovi"),
            new("ℹ️", "informazione", "info", "information"),
            new("❓", "punto interrogativo", "domanda", "question", "help"),
            new("❗", "punto esclamativo", "exclamation", "importante"),
            new("💯", "100", "cento", "pieno", "massimo"),
            new("🔴", "cerchio rosso", "red", "stop"),
            new("🟢", "cerchio verde", "green", "go", "online"),
            new("🟡", "cerchio giallo", "yellow", "warning"),
            new("🔵", "cerchio blu", "blue"),
            new("🟣", "cerchio viola", "purple"),
            new("⚫", "cerchio nero", "black"),
            new("⚪", "cerchio bianco", "white"),
            new("✨", "stelline", "magia", "sparkles", "nuovo"),
            new("💥", "esplosione", "explosion", "collision"),
            new("💫", "vertigini", "dizzy", "stordito"),
            new("🌈", "arcobaleno", "rainbow", "colori"),
            new("☀️", "sole", "sun", "giorno", "light"),
            new("🌙", "luna", "moon", "notte", "dark"),
            new("⛈️", "temporale", "storm", "pioggia"),
            new("❄️", "neve", "snow", "ghiaccio"),
            new("⚡", "fulmine", "lightning", "veloce", "elettrico"),
            new("🎵", "nota musicale", "music", "musica"),
            new("🎶", "note musicali", "musica", "melodia"),
            new("🔊", "volume alto", "sound", "audio"),
            new("🔇", "muto", "mute", "silenzioso"),

            // Frecce
            new("➡️", "freccia destra", "arrow right", "avanti"),
            new("⬅️", "freccia sinistra", "arrow left", "indietro"),
            new("⬆️", "freccia su", "arrow up", "sopra"),
            new("⬇️", "freccia giù", "arrow down", "sotto"),
            new("↗️", "freccia su-destra", "arrow up right"),
            new("↘️", "freccia giù-destra", "arrow down right"),
            new("↙️", "freccia giù-sinistra", "arrow down left"),
            new("↖️", "freccia su-sinistra", "arrow up left"),
            new("🔄", "frecce circolari", "refresh", "ricarica", "sync"),
            new("🔃", "frecce verticali", "swap", "scambia"),

            // Tech / Sviluppo
            new("🐛", "insetto", "bug", "errore"),
            new("🚀", "razzo", "rocket", "lancio", "veloce", "start"),
            new("🎨", "tavolozza", "design", "colori", "arte"),
            new("🧪", "provetta", "test", "esperimento", "lab"),
            new("🔐", "lucchetto con chiave", "secure", "autenticazione"),
            new("📡", "antenna", "satellite", "segnale", "wireless"),
            new("🛡️", "scudo", "shield", "protezione", "security"),
            new("🗑️", "cestino", "trash", "delete", "cancella"),
            new("📋", "clipboard", "appunti", "copia"),
            new("🔖", "segnalibro", "bookmark", "salva"),
            new("🏗️", "costruzione", "building", "wip", "in costruzione"),
            new("🧱", "mattone", "brick", "costruire"),
            new("🪜", "scala", "ladder"),
            new("🧲", "magnete", "magnet", "attrarre"),
            new("🎛️", "manopole", "knobs", "controlli"),
            new("🧮", "abaco", "calcolo", "math"),

            // Varie
            new("💭", "fumetto pensiero", "pensiero", "think"),
            new("🗯️", "fumetto rabbia", "anger"),
            new("💤", "zzz", "dormire", "sonno"),
            new("♻️", "riciclo", "recycle", "green"),
            new("®️", "registrato", "registered", "marchio"),
            new("©️", "copyright", "diritti"),
            new("™️", "trademark", "marchio registrato"),
            new("#️⃣", "cancelletto", "hashtag", "number"),
            new("*️⃣", "asterisco", "star key"),
            new("0️⃣", "zero", "0"),
            new("1️⃣", "uno", "1"),
            new("2️⃣", "due", "2"),
            new("3️⃣", "tre", "3"),
        };
    }

    // =============================================
    //  FONT AWESOME CATALOG (Free 6.x Solid)
    // =============================================
    private static List<IconEntry> GetFontAwesomeList()
    {
        return new List<IconEntry>
        {
            // Ui / Azioni
            new(":fa[house]", "casa / home", "home"),
            new(":fa[user]", "utente / user", "persona", "profile"),
            new(":fa[users]", "utenti / users", "gruppo", "team", "people"),
            new(":fa[gear]", "ingranaggio / settings", "opzioni", "config"),
            new(":fa[wrench]", "chiave inglese / wrench", "tools", "riparare"),
            new(":fa[screwdriver-wrench]", "cacciavite e chiave", "tools"),
            new(":fa[magnifying-glass]", "lente / search", "cerca", "trova"),
            new(":fa[plus]", "più / add", "aggiungi", "new"),
            new(":fa[minus]", "meno / remove", "rimuovi"),
            new(":fa[xmark]", "X / close", "chiudi", "cancella"),
            new(":fa[check]", "spunta / check", "ok", "done", "conferma"),
            new(":fa[ban]", "divieto / ban", "no", "stop"),
            new(":fa[trash]", "cestino / trash", "delete", "cancella"),
            new(":fa[trash-can]", "cestino / delete", "cancella"),
            new(":fa[pen]", "penna / edit", "modifica", "scrivi"),
            new(":fa[pen-to-square]", "penna quadrato / edit", "modifica"),
            new(":fa[pencil]", "matita / edit", "modifica"),
            new(":fa[copy]", "copia / copy", "duplica"),
            new(":fa[paste]", "incolla / paste", "clipboard"),
            new(":fa[clipboard]", "appunti / clipboard", "copia"),
            new(":fa[floppy-disk]", "salva / save", "salvare"),
            new(":fa[download]", "download / scarica"),
            new(":fa[upload]", "upload / carica"),
            new(":fa[arrow-right]", "freccia destra / next", "avanti", "next"),
            new(":fa[arrow-left]", "freccia sinistra / back", "indietro", "prev"),
            new(":fa[arrow-up]", "freccia su / up"),
            new(":fa[arrow-down]", "freccia giù / down"),
            new(":fa[rotate]", "ruota / refresh", "ricarica", "reload"),
            new(":fa[rotate-right]", "ruota destra / redo"),
            new(":fa[rotate-left]", "ruota sinistra / undo"),
            new(":fa[repeat]", "ripeti / repeat", "loop"),
            new(":fa[sliders]", "sliders / opzioni"),
            new(":fa[filter]", "filtro / filter"),
            new(":fa[wrench]", "chiave inglese / tool"),
            new(":fa[bug]", "bug / errore"),
            new(":fa[bug-slash]", "bug fix / risolto"),
            new(":fa[shield-halved]", "scudo / security", "sicurezza"),
            new(":fa[shield]", "scudo pieno / shield"),
            new(":fa[lock]", "lucchetto / lock", "privato"),
            new(":fa[lock-open]", "lucchetto aperto / unlock", "pubblico"),
            new(":fa[key]", "chiave / key", "password"),
            new(":fa[unlock-keyhole]", "unlock / sblocca"),
            new(":fa[link]", "link / catena", "collegamento"),
            new(":fa[link-slash]", "link rotto / broken"),
            new(":fa[paperclip]", "graffetta / clip", "allegato"),
            new(":fa[thumbtack]", "puntina / pin", "fissa"),
            new(":fa[tag]", "etichetta / tag"),
            new(":fa[tags]", "etichette / tags"),
            new(":fa[bookmark]", "segnalibro / bookmark"),
            new(":fa[print]", "stampa / print"),
            new(":fa[power-off]", "power / accensione"),
            new(":fa[bars]", "menu / hamburger", "menù"),
            new(":fa[ellipsis]", "puntini / more", "..." ),
            new(":fa[ellipsis-vertical]", "puntini verticali"),
            new(":fa[grid-2]", "griglia 2x2 / grid"),
            new(":fa[list]", "lista / list"),
            new(":fa[table-list]", "lista tabella / table list"),
            new(":fa[eye]", "occhio / view", "vedi"),
            new(":fa[eye-slash]", "occhio barrato / hide", "nascondi"),
            new(":fa[bell]", "campana / notification", "notifica"),
            new(":fa[bell-slash]", "campana muta / no notification"),
            new(":fa[envelope]", "busta / email", "mail"),
            new(":fa[envelope-open]", "busta aperta / read mail"),
            new(":fa[inbox]", "inbox / casella"),
            new(":fa[message]", "messaggio / chat"),
            new(":fa[comments]", "messaggi / comments"),
            new(":fa[comment]", "commento"),
            new(":fa[comment-dots]", "commento puntini / typing"),
            new(":fa[phone]", "telefono / phone"),
            new(":fa[mobile-screen]", "smartphone / mobile"),
            new(":fa[desktop]", "desktop / computer"),
            new(":fa[laptop]", "laptop"),
            new(":fa[tablet-screen-button]", "tablet"),
            new(":fa[server]", "server"),
            new(":fa[database]", "database / db"),
            new(":fa[hard-drive]", "hard disk / storage"),
            new(":fa[cloud]", "cloud / nuvola"),
            new(":fa[cloud-arrow-up]", "cloud upload"),
            new(":fa[cloud-arrow-down]", "cloud download"),
            new(":fa[wifi]", "wifi / rete"),
            new(":fa[bluetooth]", "bluetooth"),
            new(":fa[globe]", "globbo / web", "mondo"),
            new(":fa[location-dot]", "posizione / location", "mappa", "pin"),
            new(":fa[map]", "mappa / map"),
            new(":fa[compass]", "bussola / compass"),
            new(":fa[calendar]", "calendario / calendar"),
            new(":fa[calendar-days]", "calendario giorni"),
            new(":fa[clock]", "orologio / clock", "tempo", "ora"),
            new(":fa[hourglass]", "clessidra / hourglass"),
            new(":fa[stopwatch]", "cronometro / stopwatch"),
            new(":fa[camera]", "fotocamera / camera", "foto"),
            new(":fa[image]", "immagine / image", "foto"),
            new(":fa[images]", "immagini / images"),
            new(":fa[video]", "video"),
            new(":fa[music]", "musica / music"),
            new(":fa[headphones]", "cuffie / headphones"),
            new(":fa[volume-high]", "volume alto"),
            new(":fa[volume-xmark]", "muto / mute"),
            new(":fa[microphone]", "microfono / mic"),
            new(":fa[file]", "file / documento"),
            new(":fa[file-lines]", "file testo / document"),
            new(":fa[file-code]", "file codice / code file"),
            new(":fa[file-pdf]", "file pdf"),
            new(":fa[file-image]", "file immagine"),
            new(":fa[file-zipper]", "file zip"),
            new(":fa[folder]", "cartella / folder"),
            new(":fa[folder-open]", "cartella aperta"),
            new(":fa[folder-plus]", "nuova cartella"),
            new(":fa[code]", "codice / code"),
            new(":fa[code-branch]", "branch / git branch"),
            new(":fa[code-merge]", "merge / git merge"),
            new(":fa[code-pull-request]", "pull request"),
            new(":fa[code-commit]", "commit"),
            new(":fa[terminal]", "terminale / terminal"),
            new(":fa[circle-nodes]", "grafo / nodes"),
            new(":fa[diagram-project]", "diagramma / project", "albero"),
            new(":fa[sitemap]", "sitemap / struttura"),
            new(":fa[brain]", "cervello / brain", "ai", "ml"),
            new(":fa[robot]", "robot / ai bot"),
            new(":fa[lightbulb]", "lampadina / idea"),
            new(":fa[fire]", "fuoco / fire", "hot", "trending"),
            new(":fa[star]", "stella / star", "preferito"),
            new(":fa[heart]", "cuore / heart", "love", "like"),
            new(":fa[thumbs-up]", "pollice su / like"),
            new(":fa[thumbs-down]", "pollice giù / dislike"),
            new(":fa[hand-sparkles]", "mani magiche / clean"),
            new(":fa[handshake]", "stretta di mano"),
            new(":fa[trophy]", "trofeo / trophy"),
            new(":fa[award]", "premio / award"),
            new(":fa[medal]", "medaglia / medal"),
            new(":fa[crown]", "corona / crown"),
            new(":fa[gem]", "gioiello / gem", "premium"),
            new(":fa[diamond]", "diamante / diamond"),
            new(":fa[bullhorn]", "megafono / announce"),
            new(":fa[bolt]", "fulmine / flash", "veloce", "elettrico"),
            new(":fa[bolt-lightning]", "fulmine / lightning"),
            new(":fa[sun]", "sole / sun", "day", "light"),
            new(":fa[moon]", "luna / moon", "night", "dark"),
            new(":fa[circle-half-stroke]", "mezza luna / contrast", "tema"),
            new(":fa[palette]", "tavolozza / palette", "theme", "colori"),
            new(":fa[droplet]", "goccia / color", "colore"),
            new(":fa[brush]", "pennello / brush"),
            new(":fa[wand-magic-sparkles]", "bacchetta / magic", "automatico"),
            new(":fa[cube]", "cubo / cube", "blocco", "3d"),
            new(":fa[cubes]", "cubi / cubes"),
            new(":fa[puzzle-piece]", "puzzle / component", "plugin"),
            new(":fa[box]", "scatola / box", "package"),
            new(":fa[box-open]", "scatola aperta / unbox"),
            new(":fa[box-archive]", "archivio / archive"),
            new(":fa[cart-shopping]", "carrello / cart", "shopping"),
            new(":fa[basket-shopping]", "cestino spesa / basket"),
            new(":fa[credit-card]", "carta credito / credit card"),
            new(":fa[money-bill]", "banconota / money"),
            new(":fa[coins]", "monete / coins"),
            new(":fa[sack-dollar]", "sacco soldi / money bag"),
            new(":fa[chart-line]", "grafico linea / chart", "stats"),
            new(":fa[chart-bar]", "grafico barre / bar chart"),
            new(":fa[chart-pie]", "grafico torta / pie chart"),
            new(":fa[chart-simple]", "grafico semplice"),
            new(":fa[gauge]", "gauge / velocità", "speed"),
            new(":fa[gauge-high]", "gauge alto"),
            new(":fa[rocket]", "razzo / rocket", "lancio", "startup"),
            new(":fa[paper-plane]", "aeroplanino / send", "invia"),
            new(":fa[flag]", "bandiera / flag", "report", "segnala"),
            new(":fa[flag-checkered]", "bandiera scacchi / finish"),
            new(":fa[circle-check]", "cerchio check / verified"),
            new(":fa[circle-xmark]", "cerchio X / error"),
            new(":fa[circle-exclamation]", "cerchio ! / warning"),
            new(":fa[circle-info]", "cerchio i / info"),
            new(":fa[circle-question]", "cerchio ? / help"),
            new(":fa[triangle-exclamation]", "triangolo ! / warning"),
            new(":fa[circle-notch]", "spinner / loading"),
            new(":fa[spinner]", "spinner / loading"),
            new(":fa[certificate]", "certificato / certificate"),
            new(":fa[stamp]", "timbro / stamp", "approvato"),
            new(":fa[signature]", "firma / signature"),
        };
    }
}

/// <summary>Represents an icon entry in the picker</summary>
public class IconEntry
{
    public string Code { get; }
    public string Name { get; }
    public string Display { get; }
    public bool IsFA { get; }
    public string[] Keywords { get; }

    public IconEntry(string code, string name, params string[] keywords)
    {
        Code = code;
        Name = name;
        Keywords = keywords;
        IsFA = code.StartsWith(":fa[");
        Display = IsFA ? "🎨" : code; // Emoji rendered directly, FA shown with palette icon
    }
}
