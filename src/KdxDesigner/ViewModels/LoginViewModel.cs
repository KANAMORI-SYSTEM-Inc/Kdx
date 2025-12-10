using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KdxDesigner.Services.Authentication;
using System.Windows;

namespace KdxDesigner.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthenticationService _authService;
        private readonly IOAuthCallbackListener _callbackListener;
        private bool _mainWindowOpened = false;
        private readonly object _mainWindowLock = new object();

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private bool _isPasswordResetMode = false;

        public LoginViewModel(IAuthenticationService authService, IOAuthCallbackListener callbackListener)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _callbackListener = callbackListener ?? throw new ArgumentNullException(nameof(callbackListener));

            // 認証状態変更イベントをリッスン
            _authService.AuthStateChanged += OnAuthStateChanged;

            // 既存のセッションをチェック（少し遅延させて復元処理を待つ）
            Task.Run(async () =>
            {
                await Task.Delay(500); // セッション復元を待つ
                await CheckExistingSession();
            });
        }

        private void OnAuthStateChanged(object? sender, Supabase.Gotrue.Session? session)
        {
            if (session != null && !_mainWindowOpened)
            {
                System.Diagnostics.Debug.WriteLine("OnAuthStateChanged: Session detected, opening MainWindow...");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StatusMessage = "自動ログインに成功しました";
                    OpenMainWindow();
                });
            }
        }

        private async Task CheckExistingSession()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsLoading = true;
                    StatusMessage = "保存されたセッションを確認しています...";
                });

                var session = await _authService.GetSessionAsync();
                if (session != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        StatusMessage = "自動ログインに成功しました";
                        // 既にサインイン済みの場合、メインウィンドウを開く
                        OpenMainWindow();
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        StatusMessage = "";
                        IsLoading = false;
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Session check failed: {ex.Message}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StatusMessage = "";
                    IsLoading = false;
                });
            }
        }


        private void OpenMainWindow()
        {
            lock (_mainWindowLock)
            {
                if (_mainWindowOpened)
                {
                    System.Diagnostics.Debug.WriteLine("MainWindow already opened, skipping...");
                    return;
                }
                _mainWindowOpened = true;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                // 既存のMainViewがないか確認
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is Views.MainView)
                    {
                        System.Diagnostics.Debug.WriteLine("MainView already exists, activating it...");
                        window.Activate();
                        CloseLoginWindow();
                        return;
                    }
                }

                System.Diagnostics.Debug.WriteLine("Creating new MainView...");
                var mainWindow = new Views.MainView();
                mainWindow.Show();

                CloseLoginWindow();
            });
        }

        private void CloseLoginWindow()
        {
            // ログインウィンドウを閉じる
            foreach (Window window in Application.Current.Windows)
            {
                if (window is Views.LoginView)
                {
                    window.Close();
                    break;
                }
            }
        }

        [RelayCommand]
        private async Task SignInWithEmailAsync(System.Windows.Controls.PasswordBox passwordBox)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "メールアドレスを入力してください。";
                return;
            }

            if (passwordBox == null || string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                ErrorMessage = "パスワードを入力してください。";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;
            StatusMessage = "サインインしています...";

            try
            {
                System.Diagnostics.Debug.WriteLine($"Attempting email sign in for: {Email}");

                var success = await _authService.SignInWithEmailAsync(Email, passwordBox.Password);

                if (success)
                {
                    StatusMessage = "ログイン成功！";
                    System.Diagnostics.Debug.WriteLine("Email sign in successful");
                    await Task.Delay(500);
                    OpenMainWindow();
                }
                else
                {
                    ErrorMessage = "メールアドレスまたはパスワードが間違っています。";
                    System.Diagnostics.Debug.WriteLine("Email sign in failed - invalid credentials");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"サインインエラー: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Email sign in error: {ex}");
            }
            finally
            {
                IsLoading = false;
                StatusMessage = string.Empty;
            }
        }

        [RelayCommand]
        private async Task SignUpWithEmailAsync(System.Windows.Controls.PasswordBox passwordBox)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "メールアドレスを入力してください。";
                return;
            }

            if (passwordBox == null || string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                ErrorMessage = "パスワードを入力してください。";
                return;
            }

            // パスワードの最小長チェック
            if (passwordBox.Password.Length < 6)
            {
                ErrorMessage = "パスワードは6文字以上で入力してください。";
                return;
            }

            // TODO: 確認用パスワードの検証は後で実装
            // 現在はパスワードの基本検証のみ実装

            IsLoading = true;
            ErrorMessage = string.Empty;
            StatusMessage = "新規登録しています...";

            try
            {
                System.Diagnostics.Debug.WriteLine($"Attempting email sign up for: {Email}");

                var success = await _authService.SignUpWithEmailAsync(Email, passwordBox.Password);

                if (success)
                {
                    StatusMessage = "新規登録成功！";
                    System.Diagnostics.Debug.WriteLine("Email sign up successful");
                    await Task.Delay(500);
                    OpenMainWindow();
                }
                else
                {
                    ErrorMessage = "新規登録に失敗しました。入力内容を確認してください。";
                    System.Diagnostics.Debug.WriteLine("Email sign up failed - unknown reason");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"新規登録エラー: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Email sign up error: {ex}");
            }
            finally
            {
                IsLoading = false;
                StatusMessage = string.Empty;
            }
        }

        [RelayCommand]
        private async Task ResetPasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "メールアドレスを入力してください。";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;
            StatusMessage = "パスワードリセットメールを送信しています...";

            try
            {
                System.Diagnostics.Debug.WriteLine($"Attempting password reset for: {Email}");

                var success = await _authService.ResetPasswordForEmailAsync(Email);

                if (success)
                {
                    StatusMessage = "パスワードリセットメールを送信しました。メールをご確認ください。";
                    System.Diagnostics.Debug.WriteLine("Password reset email sent successfully");
                    IsPasswordResetMode = false;
                }
                else
                {
                    ErrorMessage = "パスワードリセットメールの送信に失敗しました。";
                    System.Diagnostics.Debug.WriteLine("Password reset failed");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"パスワードリセットエラー: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Password reset error: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void TogglePasswordResetMode()
        {
            IsPasswordResetMode = !IsPasswordResetMode;
            ErrorMessage = string.Empty;
            StatusMessage = IsPasswordResetMode
                ? "パスワードをリセットするメールアドレスを入力してください。"
                : string.Empty;
        }

        [RelayCommand]
        private void CancelPasswordReset()
        {
            IsPasswordResetMode = false;
            ErrorMessage = string.Empty;
            StatusMessage = string.Empty;
        }

        [RelayCommand]
        private async Task SignInWithGitHubAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            StatusMessage = "GitHubでサインインしています...";

            try
            {
                System.Diagnostics.Debug.WriteLine("Starting GitHub OAuth flow...");

                // OAuthコールバックリスナーを開始
                var listenerStarted = await _callbackListener.StartListenerAsync(3000);
                if (!listenerStarted)
                {
                    ErrorMessage = "認証サーバーの起動に失敗しました。ポート3000が使用中の可能性があります。";
                    return;
                }

                // GitHub OAuth URLを取得してブラウザで開く
                var authUrl = await _authService.SignInWithGitHubAsync();

                if (string.IsNullOrEmpty(authUrl))
                {
                    ErrorMessage = "GitHub認証URLの取得に失敗しました。";
                    _callbackListener.StopListener();
                    return;
                }

                StatusMessage = "ブラウザでGitHub認証を行ってください...";

                // タイムアウト付きでコールバックを待機（5分）
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

                // Implicit FlowとCode Flowの両方に対応したコールバックを待機
                var callbackResult = await _callbackListener.WaitForCallbackWithTokenAsync(cts.Token);

                if (callbackResult == null)
                {
                    ErrorMessage = "GitHub認証がキャンセルされたか、タイムアウトしました。";
                    return;
                }

                StatusMessage = "認証情報を確認しています...";

                Supabase.Gotrue.Session? session = null;

                if (callbackResult.IsImplicitFlow)
                {
                    // Implicit Flow: access_tokenとrefresh_tokenを直接使用
                    System.Diagnostics.Debug.WriteLine("Using Implicit Flow - setting session from tokens");
                    session = await _authService.SetSessionFromTokenAsync(
                        callbackResult.AccessToken!,
                        callbackResult.RefreshToken ?? string.Empty);
                }
                else if (callbackResult.IsCodeFlow)
                {
                    // Code Flow: 認証コードをセッションに交換
                    System.Diagnostics.Debug.WriteLine("Using Code Flow - exchanging code for session");
                    session = await _authService.ExchangeCodeForSessionAsync(callbackResult.Code!);
                }

                if (session != null)
                {
                    StatusMessage = "GitHubでのログインに成功しました！";
                    System.Diagnostics.Debug.WriteLine("GitHub sign in successful");
                    await Task.Delay(500);
                    OpenMainWindow();
                }
                else
                {
                    ErrorMessage = "GitHub認証に失敗しました。もう一度お試しください。";
                    System.Diagnostics.Debug.WriteLine("GitHub sign in failed - session exchange failed");
                }
            }
            catch (OperationCanceledException)
            {
                ErrorMessage = "GitHub認証がタイムアウトしました。";
                System.Diagnostics.Debug.WriteLine("GitHub sign in timed out");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"GitHubサインインエラー: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"GitHub sign in error: {ex}");
            }
            finally
            {
                _callbackListener.StopListener();
                IsLoading = false;
                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    StatusMessage = string.Empty;
                }
            }
        }
    }
}