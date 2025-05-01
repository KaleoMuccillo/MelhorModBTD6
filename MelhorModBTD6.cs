using MelonLoader;
using BTD_Mod_Helper;
using MelhorModBTD6;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Store.Loot;
using UnityEngine;
using Il2CppAssets.Scripts.Data.Behaviors.Filters;
using BTD_Mod_Helper.Api.Components;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppInterop.Runtime.Attributes;
using System;
using System.Linq;
using Il2CppAssets.Scripts.Data.MapSets;
using static MelonLoader.MelonLogger;
using JetBrains.Annotations;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Simulation.Factory;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(MelhorModBTD6.MelhorModBTD6), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace MelhorModBTD6;

public class MelhorModBTD6 : BloonsTD6Mod
{
    [RegisterTypeInIl2Cpp(false)]
    internal class SimpleMenu : MonoBehaviour
    {
    public static SimpleMenu instance;

        static RectTransform mapRect;


        public void Close()
        {
            if (gameObject)
            {
                gameObject.Destroy();
            }
        }

        public static void CreateMenu()
        {
            mapRect = InGame.instance.mapRect;

            // Cria o painel principal do menu com tamanho maior  
            var menuPanel = mapRect.gameObject.AddModHelperPanel(new("SimpleMenu", mapRect.rect.center.x, mapRect.rect.center.y, 600, 400), VanillaSprites.MainBGPanelBlue);

            instance = menuPanel.AddComponent<SimpleMenu>();

            // Adiciona o título do menu  
            var title = menuPanel.AddText(new("Title", 0, 150, 550, 100), "Adicionar Dinheiro");
            title.Text.enableAutoSizing = true;
            title.Text.fontSizeMax += 10;

            // Adiciona o campo de entrada de texto  
            var inputField = menuPanel.AddInputField(
               new Info("InputField", 0, 50, 400, 50), // Info object  
               "Digite algo...",                      // Default value  
               VanillaSprites.MainBGPanelBlue,        // Background sprite  
               null,                                  // Optional onValueChanged callback  
               42                                     // Font size (default value from ModHelperComponent.DefaultFontSize)  
            );
            inputField.Text.Text.enableAutoSizing = true;

            // Adiciona um botão acima do botão de fechar  
            var actionButton = menuPanel.AddButton(
                new Info("ActionButton", 0, -50, 200, 50),
                VanillaSprites.BlueBtn,
                new Action(() =>
                {
                    // Certifique-se de acessar o texto corretamente  
                    string inputText = inputField.Text.Text.text.Trim(); // Remove espaços extras  
                    ModHelper.Msg<MelhorModBTD6>("Texto capturado: " + inputText);

                    // Remove quaisquer caracteres não numéricos  
                    inputText = new string(inputText.Where(char.IsDigit).ToArray());
                    ModHelper.Msg<MelhorModBTD6>("Texto capturado2:" + inputText);

                    // Verifica se o texto ainda contém algo após a limpeza  
                    if (string.IsNullOrEmpty(inputText))
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado não é nulo");
                        return;
                    }

                    try
                    {
                        int cashAmount = int.Parse(inputText);
                        InGame.instance.AddCash(cashAmount);
                        ModHelper.Msg<MelhorModBTD6>("Texto digitado (convertido para número): " + cashAmount);
                    }
                    catch (FormatException)
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado não está no formato correto.");
                    }
                    catch (OverflowException)
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado é muito grande.");
                    }
                })
            );
            actionButton.AddText(new Info("ActionButtonText", 0, 0, 200, 50), "Adicionar").Text.enableAutoSizing = true;

            // Adiciona o botão de fechar  
            var closeButton = menuPanel.AddButton(
                new Info("CloseButton", 0, -150, 200, 50),
                VanillaSprites.BackBtn,
                new Action(menuPanel.gameObject.Destroy)
            );
            closeButton.AddText(new Info("CloseButtonText", 0, 0, 200, 50), "Fechar").Text.enableAutoSizing = true;
        }
    }
    internal class SimpleMenu3 : MonoBehaviour
    {
        public static SimpleMenu instance;

        static RectTransform mapRect;

        // Dicionário para armazenar os valores originais do fire rate
        private static Dictionary<string, float> originalFireRates = new();

        public static void CreateMenu3()
        {
            mapRect = InGame.instance.mapRect;

            // Cria o painel principal do menu com tamanho maior  
            var menuPanel = mapRect.gameObject.AddModHelperPanel(new("SimpleMenu3", mapRect.rect.center.x, mapRect.rect.center.y, 600, 400), VanillaSprites.MainBGPanelBlue);

            instance = menuPanel.AddComponent<SimpleMenu>();

            // Adiciona o campo de entrada de texto  
            var inputField = menuPanel.AddInputField(
               new Info("InputField", 0, 50, 400, 50), // Info object  
               "Digite algo...",                      // Default value  
               VanillaSprites.MainBGPanelBlue,        // Background sprite  
               null,                                  // Optional onValueChanged callback  
               42                                     // Font size (default value from ModHelperComponent.DefaultFontSize)  
            );
            inputField.Text.Text.enableAutoSizing = true;

            // Adiciona um botão para modificar o fire rate  
            var actionButton = menuPanel.AddButton(
                new Info("ActionButton", 0, -50, 200, 50),
                VanillaSprites.BlueBtn,
                new Action(() =>
                {
                    // Certifique-se de acessar o texto corretamente
                    string inputText = inputField.Text.Text.text.Trim(); // Remove espaços extras
                    ModHelper.Msg<MelhorModBTD6>("Texto capturado: " + inputText);

                    // Remove quaisquer caracteres não numéricos
                    inputText = new string(inputText.Where(c => char.IsDigit(c) || c == '.').ToArray());
                    ModHelper.Msg<MelhorModBTD6>("Texto capturado2: " + inputText);

                    // Verifica se o texto ainda contém algo após a limpeza
                    if (string.IsNullOrEmpty(inputText))
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado não é válido.");
                        return;
                    }

                    // Tenta converter o texto para número
                    try
                    {
                        float newFireRate = float.Parse(inputText, System.Globalization.CultureInfo.InvariantCulture);
                        ModHelper.Msg<MelhorModBTD6>("Novo Fire Rate: " + newFireRate);

                        // Aplica o novo fire rate apenas para as tropas existentes
                        foreach (var weapon in InGame.instance.GetGameModel().GetDescendants<WeaponModel>().ToList())
                        {
                            if (!originalFireRates.ContainsKey(weapon.name))
                            {
                                originalFireRates[weapon.name] = weapon.rate; // Armazena o valor original
                            }
                            weapon.rate = newFireRate; // Modifica o fire rate
                        }

                        ModHelper.Msg<MelhorModBTD6>("Fire Rate alterado para: " + newFireRate);
                    }
                    catch (FormatException)
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado não está no formato correto.");
                    }
                    catch (OverflowException)
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado é muito grande.");
                    }
                })
            );
            actionButton.AddText(new Info("ModifyButtonText", 0, 0, 200, 50), "Modificar").Text.enableAutoSizing = true;


            // Adiciona um botão para restaurar o fire rate  
            var restoreButton = menuPanel.AddButton(
                new Info("RestoreButton", 0, -150, 200, 50),
                VanillaSprites.BlueBtn,
                new Action(() =>
                {
                    // Restaura os valores originais do fire rate
                    foreach (var weapon in InGame.instance.GetGameModel().GetDescendants<WeaponModel>().ToList())
                    {
                        if (originalFireRates.ContainsKey(weapon.name))
                        {
                            weapon.rate = originalFireRates[weapon.name];
                        }
                    }

                    ModHelper.Msg<MelhorModBTD6>("Fire Rate restaurado para os valores originais.");
                })
            );
            restoreButton.AddText(new Info("RestoreButtonText", 0, 0, 200, 50), "Restaurar").Text.enableAutoSizing = true;
        

        // Adiciona o botão de fechar  
        var closeButton = menuPanel.AddButton(
                new Info("CloseButton", 0, -250, 200, 50),
                VanillaSprites.BackBtn,
                new Action(menuPanel.gameObject.Destroy)
            );
            closeButton.AddText(new Info("CloseButtonText", 0, 0, 200, 50), "Fechar").Text.enableAutoSizing = true;
        }
    }

    internal class SimpleMenu2 : MonoBehaviour
    {
        public static SimpleMenu instance;

        static RectTransform mapRect;

        public void Close()
        {
            if (gameObject)
            {
                gameObject.Destroy();
            }
        }

        public static void CreateMenu2()
        {
            mapRect = InGame.instance.mapRect;

            // Cria o painel principal do menu com tamanho maior  
            var menuPanel = mapRect.gameObject.AddModHelperPanel(new("SimpleMenu", mapRect.rect.center.x, mapRect.rect.center.y, 600, 400), VanillaSprites.MainBGPanelBlue);

            instance = menuPanel.AddComponent<SimpleMenu>();

            // Adiciona o título do menu  
            var title = menuPanel.AddText(new("Title", 0, 150, 550, 100), "Adicionar Vida");
            title.Text.enableAutoSizing = true;
            title.Text.fontSizeMax += 10;

            // Adiciona o campo de entrada de texto  
            var inputField = menuPanel.AddInputField(
               new Info("InputField", 0, 50, 400, 50), // Info object  
               "Digite algo...",                      // Default value  
               VanillaSprites.MainBGPanelBlue,        // Background sprite  
               null,                                  // Optional onValueChanged callback  
               42                                     // Font size (default value from ModHelperComponent.DefaultFontSize)  
            );
            inputField.Text.Text.enableAutoSizing = true;

            // Adiciona um botão acima do botão de fechar  
            var actionButton = menuPanel.AddButton(
                new Info("ActionButton", 0, -50, 200, 50),
                VanillaSprites.BlueBtn,
                new Action(() =>
                {
                    // Certifique-se de acessar o texto corretamente
                    string inputText = inputField.Text.Text.text.Trim(); // Remove espaços extras
                    ModHelper.Msg<MelhorModBTD6>("Texto capturado: " + inputText);

                    // Remove quaisquer caracteres não numéricos
                    inputText = new string(inputText.Where(char.IsDigit).ToArray());
                    ModHelper.Msg<MelhorModBTD6>("Texto capturado2:" + inputText);

                    // Verifica se o texto ainda contém algo após a limpeza
                    if (string.IsNullOrEmpty(inputText))
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado não é nulo");
                        return;
                    }

                    // Tenta converter o texto para número
                    try
                    {
                        int healthAmount = int.Parse(inputText);
                        InGame.instance.AddHealth(healthAmount);
                        ModHelper.Msg<MelhorModBTD6>("Texto digitado (convertido para número): " + healthAmount);
                    }
                    catch (FormatException)
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado não está no formato correto.");
                    }
                    catch (OverflowException)
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado é muito grande.");
                    }
                })
            );
            actionButton.AddText(new Info("ActionButtonText", 0, 0, 200, 50), "Adicionar").Text.enableAutoSizing = true;

            // Adiciona um botão para setar a vida
            var setHealthButton = menuPanel.AddButton(
                new Info("SetHealthButton", 0, -100, 200, 50), // Posição abaixo do botão de adicionar vida
                VanillaSprites.BlueBtn,
                new Action(() =>
                {
                    // Certifique-se de acessar o texto corretamente
                    string inputText = inputField.Text.Text.text.Trim(); // Remove espaços extras
                    ModHelper.Msg<MelhorModBTD6>("Texto capturado: " + inputText);

                    // Remove quaisquer caracteres não numéricos
                    inputText = new string(inputText.Where(char.IsDigit).ToArray());
                    ModHelper.Msg<MelhorModBTD6>("Texto capturado2:" + inputText);

                    // Verifica se o texto ainda contém algo após a limpeza
                    if (string.IsNullOrEmpty(inputText))
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado não é válido.");
                        return;
                    }

                    // Tenta converter o texto para número
                    try
                    {
                        int healthAmount = int.Parse(inputText);
                        InGame.instance.SetHealth(healthAmount); // Define a vida do jogador
                        ModHelper.Msg<MelhorModBTD6>("Vida setada para: " + healthAmount);
                    }
                    catch (FormatException)
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado não está no formato correto.");
                    }
                    catch (OverflowException)
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado é muito grande.");
                    }
                })
            );
            setHealthButton.AddText(new Info("SetHealthButtonText", 0, 0, 200, 50), "Setar Vida").Text.enableAutoSizing = true;


            // Adiciona o botão de fechar  
            var closeButton = menuPanel.AddButton(
                new Info("CloseButton", 0, -150, 200, 50),
                VanillaSprites.BackBtn,
                new Action(menuPanel.gameObject.Destroy)
            );
            closeButton.AddText(new Info("CloseButtonText", 0, 0, 200, 50), "Fechar").Text.enableAutoSizing = true;
        }
    }

    internal class SimpleMenu4 : MonoBehaviour
    {
        public static SimpleMenu instance;

        static RectTransform mapRect;

        public void Close()
        {
            if (gameObject)
            {
                gameObject.Destroy();
            }
        }

        public static void CreateMenu4()
        {
            mapRect = InGame.instance.mapRect;

            // Cria o painel principal do menu com tamanho maior  
            var menuPanel = mapRect.gameObject.AddModHelperPanel(new("SimpleMenu", mapRect.rect.center.x, mapRect.rect.center.y, 600, 400), VanillaSprites.MainBGPanelBlue);

            instance = menuPanel.AddComponent<SimpleMenu>();

            // Adiciona o título do menu  
            var title = menuPanel.AddText(new("Title", 0, 150, 550, 100), "Setar Round");
            title.Text.enableAutoSizing = true;
            title.Text.fontSizeMax += 10;

            // Adiciona o campo de entrada de texto  
            var inputField = menuPanel.AddInputField(
               new Info("InputField", 0, 50, 400, 50), // Info object  
               "Digite algo...",                      // Default value  
               VanillaSprites.MainBGPanelBlue,        // Background sprite  
               null,                                  // Optional onValueChanged callback  
               42                                     // Font size (default value from ModHelperComponent.DefaultFontSize)  
            );
            inputField.Text.Text.enableAutoSizing = true;

            // Adiciona um botão acima do botão de fechar  
            var actionButton = menuPanel.AddButton(
                new Info("ActionButton", 0, -50, 200, 50),
                VanillaSprites.BlueBtn,
                new Action(() =>
                {
                    // Certifique-se de acessar o texto corretamente
                    string inputText = inputField.Text.Text.text.Trim(); // Remove espaços extras
                    ModHelper.Msg<MelhorModBTD6>("Texto capturado: " + inputText);

                    // Remove quaisquer caracteres não numéricos
                    inputText = new string(inputText.Where(char.IsDigit).ToArray());
                    ModHelper.Msg<MelhorModBTD6>("Texto capturado2:" + inputText);

                    // Verifica se o texto ainda contém algo após a limpeza
                    if (string.IsNullOrEmpty(inputText))
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado não é nulo");
                        return;
                    }

                    // Tenta converter o texto para número
                    try
                    {
                        int round= int.Parse(inputText);
                        InGame.instance.SetRound(round - 1);
                        ModHelper.Msg<MelhorModBTD6>("Texto digitado (convertido para número): " + round);
                    }
                    catch (FormatException)
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado não está no formato correto.");
                    }
                    catch (OverflowException)
                    {
                        ModHelper.Msg<MelhorModBTD6>("Erro: O valor digitado é muito grande.");
                    }
                })
            );
            actionButton.AddText(new Info("ActionButtonText", 0, 0, 200, 50), "Setar").Text.enableAutoSizing = true;


            // Adiciona o botão de fechar  
            var closeButton = menuPanel.AddButton(
                new Info("CloseButton", 0, -150, 200, 50),
                VanillaSprites.BackBtn,
                new Action(menuPanel.gameObject.Destroy)
            );
            closeButton.AddText(new Info("CloseButtonText", 0, 0, 200, 50), "Fechar").Text.enableAutoSizing = true;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // Verifica se o jogador está dentro de uma partida
        bool InAGame = InGame.instance != null && InGame.instance.bridge != null;
        if (InAGame) // Pressione Numpad 1 para abrir o menu
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                if (SimpleMenu.instance == null)
                {
                    SimpleMenu.CreateMenu();
                }
                else
                {
                    SimpleMenu.instance.Close();
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                if (SimpleMenu2.instance == null)
                {
                    SimpleMenu2.CreateMenu2();
                }
                else
                {
                    SimpleMenu2.instance.Close();
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                if (SimpleMenu3.instance == null)
                {
                    SimpleMenu3.CreateMenu3();
                }
                else
                {
                    SimpleMenu3.instance.Close();
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                if (SimpleMenu4.instance == null)
                {
                    SimpleMenu4.CreateMenu4();
                }
                else
                {
                    SimpleMenu4.instance.Close();
                }
            }
        }
    }
}

