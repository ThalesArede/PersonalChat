using PersonalAryChat.Model;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;

namespace PersonalAryChat
{
    public partial class MainWindow : Window
    {
        private readonly string pastaDocumentosLocal =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents");

        private string CaminhoHistorico =>
            Path.Combine(pastaDocumentosLocal, "historico_chat.json");

        private List<Mensagem> mensagens = new();

        public MainWindow()
        {
            InitializeComponent();
            CarregarHistorico();
            RenderizarHistorico();
        }

        private string GerarRespostaDemo(string pergunta)
        {
            return "Esta é uma resposta de demonstração do seu assistente local. Em breve, isso será substituído pela resposta da IA real.";
        }

        private async void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtMensagem.Text))
            {
                string textoUsuario = TxtMensagem.Text;
                AdicionarMensagem(textoUsuario, true);
                TxtMensagem.Clear();

                await Task.Delay(600);

                string respostaDemo = GerarRespostaDemo(textoUsuario);
                AdicionarMensagem(respostaDemo, false);
            }
        }

        private void AdicionarMensagem(string texto, bool ehUsuario)
        {
            var mensagem = new Mensagem
            {
                Texto = texto,
                EhUsuario = ehUsuario,
                Horario = DateTime.Now
            };

            mensagens.Add(mensagem);
            SalvarHistorico();
            RenderizarMensagem(mensagem, autoScroll: true);
        }


        private void RenderizarMensagem(Mensagem mensagem, bool autoScroll = false)
        {
            StackPanel container = new StackPanel
            {
                Margin = new Thickness(10),
                HorizontalAlignment = mensagem.EhUsuario ? HorizontalAlignment.Right : HorizontalAlignment.Left
            };

            Border bubble = new Border
            {
                Background = mensagem.EhUsuario ? Brushes.Red : Brushes.DimGray,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(12),
                MaxWidth = 260
            };

            TextBlock txt = new TextBlock
            {
                Text = mensagem.Texto,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                FontFamily = new FontFamily("pack://application:,,,/Fonts/#Montserrat"),
                FontSize = 13
            };

            TextBlock hora = new TextBlock
            {
                Text = mensagem.Horario.ToString("dd/MM HH:mm"),
                FontSize = 10,
                Foreground = Brushes.LightGray,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 2, 2, 0),
                FontFamily = new FontFamily("pack://application:,,,/Fonts/#Montserrat")
            };

            bubble.Child = txt;

            container.Children.Add(bubble);
            container.Children.Add(hora);

            ChatPanel.Children.Add(container);

            if (autoScroll)
                ChatScroll.ScrollToEnd();
        }

        private void RenderizarHistorico()
        {
            ChatPanel.Children.Clear();

            foreach (var msg in mensagens)
                RenderizarMensagem(msg);

            ChatScroll.ScrollToEnd();
        }

        private void SalvarHistorico()
        {
            Directory.CreateDirectory(pastaDocumentosLocal);

            var json = JsonSerializer.Serialize(mensagens);
            File.WriteAllText(CaminhoHistorico, json);
        }

        private void CarregarHistorico()
        {
            if (File.Exists(CaminhoHistorico))
            {
                var json = File.ReadAllText(CaminhoHistorico);
                mensagens = JsonSerializer.Deserialize<List<Mensagem>>(json) ?? new();
            }
        }

        private void TxtMensagem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnEnviar_Click(sender, e);
                e.Handled = true;
            }
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is Button))
            {
                try { this.DragMove(); } catch { }
            }
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.CreateDirectory(pastaDocumentosLocal);
                string caminhoExportacao = Path.Combine(pastaDocumentosLocal, "chat_exportado.txt");

                if (mensagens.Count == 0)
                {
                    MessageBox.Show("Não há mensagens para exportar.", "Aviso",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                List<string> linhas = new();
                foreach (var msg in mensagens)
                {
                    string prefixo = msg.EhUsuario ? "Você" : "Assistente";
                    string horario = msg.Horario.ToString("dd/MM HH:mm");
                    linhas.Add($"({horario}) {prefixo}: {msg.Texto}");
                }


                File.WriteAllLines(caminhoExportacao, linhas);

                MessageBox.Show($"Chat exportado com sucesso para:\n{caminhoExportacao}",
                                "Exportação concluída",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            } catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar o chat:\n{ex.Message}",
                                "Erro",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void BtnConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Em desenvolvimento.");
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            mensagens.Clear();
            ChatPanel.Children.Clear();

            if (File.Exists(CaminhoHistorico))
                File.WriteAllText(CaminhoHistorico, "[]");
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnMinimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
