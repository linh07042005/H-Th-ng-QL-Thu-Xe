
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.Text.RegularExpressions;
using CarRentalManagement.BLL.Services;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.Presentation
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
            this.Load += FrmLogin_Load;
        }

        private async void FrmLogin_Load(object sender, EventArgs e)
        {
            try
            {
                webView21.Dock = DockStyle.Fill;

                var options = new CoreWebView2EnvironmentOptions(
                    "--allow-file-access-from-files --autoplay-policy=no-user-gesture-required"
                );

                var env = await CoreWebView2Environment.CreateAsync(
                    null, null, options
                );

                await webView21.EnsureCoreWebView2Async(env);

                string videoPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "Videos", "luxury-car.mp4"
                );

                string htmlPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "login_bg.html"
                );

                string videoUri = new Uri(videoPath).AbsoluteUri;

                string htmlContent = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
<meta charset='utf-8' />

<style>
* {{
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: 'Segoe UI', sans-serif;
}}

body {{
    background-color: #000;
    overflow: hidden;
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
}}

#bg-video {{
    position: fixed;
    right: 0;
    bottom: 0;
    min-width: 100%;
    min-height: 100%;
    object-fit: cover;
    z-index: -1;
    filter: brightness(45%);
}}

.login-card {{
    position: relative;
    background: rgba(18, 18, 18, 0.7);
    backdrop-filter: blur(18px);
    -webkit-backdrop-filter: blur(18px);
    border: 1px solid rgba(198, 166, 107, 0.4);
    border-radius: 20px;
    padding: 35px;
    width: 410px;
    text-align: center;
    box-shadow: 0 15px 35px rgba(0,0,0,0.8),
                0 0 20px rgba(198, 166, 107, 0.15);
    overflow: hidden;
}}

.card-corner {{
    position: absolute;
    width: 20px;
    height: 20px;
    border: 2px solid #C6A66B;
}}

.corner-tl {{
    top: 10px;
    left: 10px;
    border-right: none;
    border-bottom: none;
}}

.corner-tr {{
    top: 10px;
    right: 10px;
    border-left: none;
    border-bottom: none;
}}

.corner-bl {{
    bottom: 10px;
    left: 10px;
    border-right: none;
    border-top: none;
}}

.corner-br {{
    bottom: 10px;
    right: 10px;
    border-left: none;
    border-top: none;
}}

.brand-header {{
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 12px;
    margin-top: 5px;
}}

.crown-car-logo {{
    width: 75px;
    height: 45px;
    filter: drop-shadow(0 0 8px rgba(212, 175, 55, 0.4));
}}

.brand-title-box {{
    text-align: left;
}}

.brand-title {{
    color: #C6A66B;
    font-size: 22px;
    font-weight: 800;
    letter-spacing: 3px;
    text-transform: uppercase;
    line-height: 1.1;
}}

.brand-sub {{
    color: #888;
    font-size: 9px;
    letter-spacing: 1.5px;
    margin-top: 3px;
}}

.divider {{
    height: 1px;
    background: linear-gradient(
        90deg,
        transparent,
        rgba(198, 166, 107, 0.5),
        transparent
    );
    margin: 20px 0;
}}

.welcome-text {{
    color: #FFF;
    font-size: 14px;
    font-weight: 700;
    letter-spacing: 2px;
}}

.portal-text {{
    color: #999;
    font-size: 10px;
    margin-bottom: 20px;
    letter-spacing: 1px;
}}

.input-group {{
    text-align: left;
    margin-bottom: 16px;
    position: relative;
}}

.input-group label {{
    display: block;
    color: #C6A66B;
    font-size: 11px;
    font-weight: 700;
    letter-spacing: 1px;
    margin-bottom: 6px;
}}

.input-wrapper {{
    position: relative;
}}

.input-group input {{
    width: 100%;
    padding: 12px 15px;
    background: rgba(0, 0, 0, 0.6);
    border: 1px solid rgba(198, 166, 107, 0.3);
    border-radius: 8px;
    color: #fff;
    font-size: 13px;
    outline: none;
    transition: 0.3s;
}}

.input-group input:focus {{
    border-color: #D4AF37;
    box-shadow: 0 0 10px rgba(212, 175, 55, 0.4);
}}

.btn-show {{
    position: absolute;
    right: 12px;
    top: 50%;
    transform: translateY(-50%);
    color: #888;
    font-size: 11px;
    cursor: pointer;
    font-weight: 600;
    user-select: none;
    transition: 0.2s;
}}

.btn-show:hover {{
    color: #C6A66B;
}}

.btn-signin {{
    width: 100%;
    padding: 14px;
    margin-top: 10px;
    background: linear-gradient(135deg, #D4AF37 0%, #AA7C11 100%);
    border: none;
    border-radius: 8px;
    color: #000;
    font-weight: 800;
    font-size: 13px;
    letter-spacing: 1px;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: 0 4px 15px rgba(212, 175, 55, 0.3);
}}

.btn-signin:hover {{
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(212, 175, 55, 0.6);
    background: linear-gradient(135deg, #E5C158 0%, #C69214 100%);
}}

.footer-text {{
    margin-top: 22px;
    color: #777;
    font-size: 9px;
    letter-spacing: 1.5px;
}}
</style>
</head>

<body>

<video autoplay loop muted playsinline id='bg-video'>
    <source src='{videoUri}' type='video/mp4'>
</video>

<div class='login-card'>

    <div class='card-corner corner-tl'></div>
    <div class='card-corner corner-tr'></div>
    <div class='card-corner corner-bl'></div>
    <div class='card-corner corner-br'></div>

    <div class='brand-header'>

        <div class='crown-car-logo'>
            <svg viewBox='0 0 500 250' width='100%' height='100%'>
                <defs>
                    <linearGradient id='gold-grad'
                        x1='0%' y1='0%' x2='100%' y2='100%'>
                        <stop offset='0%' stop-color='#FFE599' />
                        <stop offset='50%' stop-color='#D4AF37' />
                        <stop offset='100%' stop-color='#AA7C11' />
                    </linearGradient>
                </defs>

                <path d='M 50 180 C 70 170 90 145 140 135 C 180 125 210 95 270 95 C 330 95 380 115 410 130 C 440 145 470 155 490 155'
                    fill='none'
                    stroke='url(#gold-grad)'
                    stroke-width='8'
                    stroke-linecap='round'/>

                <path d='M 60 195 L 470 195 C 485 195 490 185 475 175 C 430 145 370 135 320 135 L 200 135 C 150 135 100 160 60 195 Z'
                    fill='none'
                    stroke='url(#gold-grad)'
                    stroke-width='6'/>

                <path d='M 110 195 C 110 160 160 160 160 195'
                    fill='none'
                    stroke='url(#gold-grad)'
                    stroke-width='7'/>

                <path d='M 310 115 L 325 70 L 350 95 L 390 40 L 420 90 L 455 55 L 465 125 Z'
                    fill='url(#gold-grad)'
                    opacity='0.9'/>
            </svg>
        </div>

        <div class='brand-title-box'>
            <div class='brand-title'>LINHXURY</div>
            <div class='brand-sub'>HỆ THỐNG QUẢN LÝ CHO THUÊ XE</div>
        </div>

    </div>

    <div class='divider'></div>

    <div class='welcome-text'>ĐĂNG NHẬP HỆ THỐNG</div>
    <div class='portal-text'>DÀNH CHO QUẢN TRỊ VIÊN & NHÂN VIÊN</div>

    <div class='input-group'>
        <label>TÊN ĐĂNG NHẬP</label>
        <input type='text'
            id='txtUser'
            placeholder='Nhập tên đăng nhập'
            autocomplete='off'>
    </div>

    <div class='input-group'>
        <label>MẬT KHẨU</label>

        <div class='input-wrapper'>
            <input type='password'
                id='txtPass'
                placeholder='Nhập mật khẩu'>

            <span class='btn-show' onclick='togglePass()'>HIỆN</span>
        </div>
    </div>

    <button class='btn-signin' onclick='handleLogin()'>
        ĐĂNG NHẬP
    </button>

    <div class='footer-text'>
        SANG TRỌNG • TIỆN NGHI • ĐẲNG CẤP
    </div>

</div>

<script>

function togglePass()
{{
    var passInput = document.getElementById('txtPass');
    var btnShow = document.querySelector('.btn-show');

    if (passInput.type === 'password')
    {{
        passInput.type = 'text';
        btnShow.innerText = 'ẨN';
    }}
    else
    {{
        passInput.type = 'password';
        btnShow.innerText = 'HIỆN';
    }}
}}

function handleLogin()
{{
    var user = document.getElementById('txtUser').value;
    var pass = document.getElementById('txtPass').value;

    window.chrome.webview.postMessage(JSON.stringify({{
        action: 'login',
        username: user,
        password: pass
    }}));
}}

</script>

</body>
</html>";

                File.WriteAllText(
                    htmlPath,
                    htmlContent,
                    Encoding.UTF8
                );

                webView21.CoreWebView2.Navigate(
                    new Uri(htmlPath).AbsoluteUri
                );

                webView21.CoreWebView2.WebMessageReceived +=
                    CoreWebView2_WebMessageReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải giao diện đăng nhập: " + ex.Message,
                    "Lỗi khởi động",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void CoreWebView2_WebMessageReceived(
            object sender,
            CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string json = e.TryGetWebMessageAsString();

                Match actionMatch = Regex.Match(
                    json,
                    @"""action""\s*:\s*""([^""]*)"""
                );

                Match userMatch = Regex.Match(
                    json,
                    @"""username""\s*:\s*""((?:\\.|[^""\\])*)"""
                );

                Match passMatch = Regex.Match(
                    json,
                    @"""password""\s*:\s*""((?:\\.|[^""\\])*)"""
                );

                if (!actionMatch.Success ||
                    actionMatch.Groups[1].Value != "login")
                {
                    return;
                }

                if (!userMatch.Success || !passMatch.Success)
                {
                    throw new Exception(
                        "Thông tin đăng nhập không hợp lệ!"
                    );
                }

                string username = Regex.Unescape(
                    userMatch.Groups[1].Value
                );

                string password = Regex.Unescape(
                    passMatch.Groups[1].Value
                );

                LoginService loginService = new LoginService();

                User loggedInUser = loginService.Login(
                    username,
                    password
                );

                MessageBox.Show(
                    "Đăng nhập thành công!",
                    "LINHXURY",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                FrmMain mainForm = new FrmMain(loggedInUser);

                mainForm.FormClosed += (s, args) => this.Close();

                this.Hide();
                mainForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Đăng nhập thất bại",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void webView21_Click(object sender, EventArgs e)
        {
        }
    }
}
