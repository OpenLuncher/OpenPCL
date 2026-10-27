Friend Module ModSecret

    '标注 PCL 的不同分支，仅用于替换标记
    Public Const VersionBranchMain As String = "Official"
    '注册表中用于存放 PCL 设置的文件夹名
    Public Const RegFolder As String = "PCL"
    '用于微软登录的 ClientId
    Public Const OAuthClientId As String = "fe72edc2-3a6f-4280-90e8-e2beb64ce7e1"
    '用于匿名数据收集的腾讯云日志服务上报 URL，形如 https://{region}.cls.tencentcs.com/track?topic_id={topic_id}
    Public Const ClsBaseUrl As String = "https://ap-shanghai.cls.tencentcs.com/track?topic_id=pcl-remote-1253424809"

#Region "网络鉴权"

    Public Const RestrictMcDownload As Boolean = True

    ''' <summary>
    ''' 为带有 {CDN} 标记的 URL 生成 CDN 签名。
    ''' 若 URL 不带该标记，则原样返回。
    ''' </summary>
    Friend Function SecretCdnSign(UrlWithMark As String) As String
        If ApplicationStartTick < 1 Then Return UrlWithMark
        If Not UrlWithMark.EndsWithF("{CDN}") Then Return UrlWithMark
        Dim Url As String = UrlWithMark.Replace("{CDN}", "").Replace(" ", "%20")
        Dim RandomMark As String = RandomInteger(0, 2147483645).ToString("x").EnsureLength("0"c, 8)
        Dim Timestamp As String = GetUnixTimestampUtc().ToString()
        Dim UrlPath As String = Url.Substring(Url.IndexOfF("://") + 3)
        UrlPath = UrlPath.Substring(UrlPath.IndexOfF("/"))
        Dim Sign As String = CryptographyUtils.ComputeHash({UrlPath, Timestamp, RandomMark, "0", "PCL2Server"}.Join("-"), CryptographyUtils.HashMethod.Md5)
        Return Url & If(Url.Contains("?"), "&", "?") & "sign=" & {Timestamp, RandomMark, "0", Sign}.Join("-")
    End Function

    ''' <summary>
    ''' 设置 Headers 的 UA、Referer。
    ''' </summary>
    Friend Sub SecretHeadersSign(Url As String, ByRef Req As HttpRequestMessage, Optional SimulateBrowserHeaders As Boolean = False)
        If ApplicationStartTick < 1 Then Return
        If Not Req.Headers.UserAgent.Any Then
            If Url.Contains("baidupcs.com") OrElse Url.Contains("baidu.com") Then
                Req.Headers.Add("User-Agent", "LogStatistic")  '#4951
            ElseIf SimulateBrowserHeaders Then
                Req.Headers.Add("User-Agent", $"PCL2/{VersionBaseName}.{CInt(BuildType)} Mozilla/5.0 AppleWebKit/537.36 Chrome/63.0.3239.132 Safari/537.36")
            Else
                Req.Headers.Add("User-Agent", $"PCL2/{VersionBaseName}.{CInt(BuildType)}")
            End If
        End If
        If Not SimulateBrowserHeaders Then Req.Headers.Referrer = New Uri($"http://{VersionCode}.pcl2.server/")
        If Url.Contains("api.curseforge.com") OrElse Url.Contains("forgecdn.net") Then Req.Headers.Add("x-api-key", Encoding.Default.GetString(Convert.FromBase64String("JDJhJDEwJDdXV1YxV0VMY3dYOFhwN2Q2OE1Icy41Z3JUeVpWYTZTeW55ZVN5TWgxcnNFODluSWEwOXpP")))
    End Sub

#End Region

#Region "主题"

    Public Color1 As New MyColor(52, 61, 74)
    Public Color2 As New MyColor(11, 91, 203)
    Public Color3 As New MyColor(19, 112, 243)
    Public Color4 As New MyColor(72, 144, 245)
    Public Color5 As New MyColor(150, 192, 249)
    Public Color6 As New MyColor(213, 230, 253)
    Public Color7 As New MyColor(222, 236, 253)
    Public Color8 As New MyColor(234, 242, 254)
    Public ColorBg0 As New MyColor(150, 192, 249)
    Public ColorBg1 As New MyColor(190, Color7)
    Public ColorGray1 As New MyColor(64, 64, 64)
    Public ColorGray2 As New MyColor(115, 115, 115)
    Public ColorGray3 As New MyColor(140, 140, 140)
    Public ColorGray4 As New MyColor(166, 166, 166)
    Public ColorGray5 As New MyColor(204, 204, 204)
    Public ColorGray6 As New MyColor(235, 235, 235)
    Public ColorGray7 As New MyColor(240, 240, 240)
    Public ColorGray8 As New MyColor(245, 245, 245)
    Public ColorSemiTransparent As New MyColor(1, Color8)

    Public ThemeNow As Integer = -1
    Public ColorHue As Integer = 210, ColorSat As Integer = 85, ColorLightAdjust As Integer = 0, ColorHueTopbarDelta As OneOf(Of Integer, Integer()) = 0
    Public ThemeDontClick As Integer = 0
    Private IsThemeColorfulLoaded As Boolean = False
    Private ColorHueDelta As Integer = 1

    Public Sub ThemeRefresh(Optional NewTheme As Integer = -1)
        Try
            If (ThemeNow <> NewTheme OrElse NewTheme < 0) AndAlso ThemeCheckOne(If(NewTheme >= 0, NewTheme, ThemeNow)) Then
                If NewTheme >= 0 Then ThemeNow = NewTheme

                Select Case ThemeNow
                    Case 10
                        ColorHue = 5
                        ColorSat = 70
                        ColorLightAdjust = -5
                        ColorHueTopbarDelta = 0
                    Case 9
                        ColorHue = 25
                        ColorSat = 75
                        ColorLightAdjust = 5
                        ColorHueTopbarDelta = 0
                    Case 4
                        ColorHue = 35
                        ColorSat = 30
                        ColorLightAdjust = 0
                        ColorHueTopbarDelta = 0
                    Case 8
                        ColorHue = 50
                        ColorSat = 65
                        ColorLightAdjust = 10
                        ColorHueTopbarDelta = 0
                    Case 3
                        ColorHue = 75
                        ColorSat = 50
                        ColorLightAdjust = 0
                        ColorHueTopbarDelta = 0
                    Case 2
                        ColorHue = 130
                        ColorSat = 40
                        ColorLightAdjust = 0
                        ColorHueTopbarDelta = {5, -5, 5}
                    Case 42
                        ColorHue = 225
                        ColorSat = 60
                        ColorLightAdjust = -15
                        ColorHueTopbarDelta = 0
                    Case 1
                        ColorHue = 180
                        ColorSat = 50
                        ColorLightAdjust = 0
                        ColorHueTopbarDelta = 0
                    Case 0
                        ColorHue = 210
                        ColorSat = 85
                        ColorLightAdjust = 0
                        ColorHueTopbarDelta = 0
                    Case 5
                        ColorHue = 225
                        ColorSat = 10
                        ColorLightAdjust = -20
                        ColorHueTopbarDelta = 0
                    Case 11
                        ColorHue = 230
                        ColorSat = 60
                        ColorLightAdjust = -5
                        ColorHueTopbarDelta = 0
                    Case 7
                        ColorHue = 285
                        ColorSat = 50
                        ColorLightAdjust = 0
                        ColorHueTopbarDelta = 0
                    Case 13
                        ColorHue = 290
                        ColorSat = 50
                        ColorLightAdjust = 0
                        ColorHueTopbarDelta = {-30, 10, 50}
                    Case 6
                        ColorHue = 345
                        ColorSat = 70
                        ColorLightAdjust = 15
                        ColorHueTopbarDelta = 0
                    Case 12
                        If Not IsThemeColorfulLoaded Then
                            IsThemeColorfulLoaded = True
                            ColorHue = RandomInteger(0, 359)
                            ColorHueDelta = (RandomInteger(0, 1) * 2 - 1) * 4
                        End If
                        ColorHue = (ColorHue + ColorHueDelta) Mod 360
                        ColorSat = 45
                        ColorLightAdjust = 0
                    Case 14
                        ColorHue = Settings.Get(Of Integer)("UiLauncherHue")
                        ColorSat = Settings.Get(Of Integer)("UiLauncherSat")
                        ColorLightAdjust = Settings.Get(Of Integer)("UiLauncherLight") - 20
                        ColorHueTopbarDelta = Settings.Get(Of Integer)("UiLauncherDelta") - 90
                    Case Else
                        ColorHue = 210
                        ColorSat = 85
                        ColorLightAdjust = 0
                End Select
                Select Case ThemeDontClick
                    Case 1
                        ColorLightAdjust = 999
                    Case 2
                        ColorHue = RandomInteger(0, 359)
                        ColorSat = RandomInteger(40, 70)
                        ColorLightAdjust = RandomInteger(-20, 20)
                End Select
                Color1 = New MyColor().FromHSL2(ColorHue, ColorSat * 0.2, 25 + ColorLightAdjust * 0.3)
                Color2 = New MyColor().FromHSL2(ColorHue, ColorSat, 45 + ColorLightAdjust)
                Color3 = New MyColor().FromHSL2(ColorHue, ColorSat, 55 + ColorLightAdjust)
                Color4 = New MyColor().FromHSL2(ColorHue, ColorSat, 65 + ColorLightAdjust)
                Color5 = New MyColor().FromHSL2(ColorHue, ColorSat, 80 + ColorLightAdjust * 0.4)
                Color6 = New MyColor().FromHSL2(ColorHue, ColorSat, 91 + ColorLightAdjust * 0.1)
                Color7 = New MyColor().FromHSL2(ColorHue, ColorSat, 95)
                Color8 = New MyColor().FromHSL2(ColorHue, ColorSat, 97)
                ColorBg0 = Color4 * 0.4 + Color5 * 0.4 + ColorGray4 * 0.2
                ColorBg1 = New MyColor(190, Color7)

                ColorSemiTransparent = New MyColor(1, Color8)
                Application.Current.Resources("ColorBrush1") = New SolidColorBrush(Color1)
                Application.Current.Resources("ColorBrush2") = New SolidColorBrush(Color2)
                Application.Current.Resources("ColorBrush3") = New SolidColorBrush(Color3)
                Application.Current.Resources("ColorBrush4") = New SolidColorBrush(Color4)
                Application.Current.Resources("ColorBrush5") = New SolidColorBrush(Color5)
                Application.Current.Resources("ColorBrush6") = New SolidColorBrush(Color6)
                Application.Current.Resources("ColorBrush7") = New SolidColorBrush(Color7)
                Application.Current.Resources("ColorBrush8") = New SolidColorBrush(Color8)
                Application.Current.Resources("ColorBrushBg0") = New SolidColorBrush(ColorBg0)
                Application.Current.Resources("ColorBrushBg1") = New SolidColorBrush(ColorBg1)
                Application.Current.Resources("ColorObject1") = CType(Color1, Color)
                Application.Current.Resources("ColorObject2") = CType(Color2, Color)
                Application.Current.Resources("ColorObject3") = CType(Color3, Color)
                Application.Current.Resources("ColorObject4") = CType(Color4, Color)
                Application.Current.Resources("ColorObject5") = CType(Color5, Color)
                Application.Current.Resources("ColorObject6") = CType(Color6, Color)
                Application.Current.Resources("ColorObject7") = CType(Color7, Color)
                Application.Current.Resources("ColorObject8") = CType(Color8, Color)
                Application.Current.Resources("ColorObjectBg0") = CType(ColorBg0, Color)
                Application.Current.Resources("ColorObjectBg1") = CType(ColorBg1, Color)
                ThemeRefreshMain()
                If ThemeNow <> 12 AndAlso ThemeNow <> 14 AndAlso ThemeDontClick <> 2 Then
                    Logger.Info($"刷新主题：{ThemeNow}")
                End If
            End If
        Catch ex As Exception
            Logger.Error(ex, "刷新主题颜色失败", LogBehavior.Toast)
        End Try
    End Sub
    Public Sub ThemeRefreshMain()
        RunInUi(
        Sub()
            If Not FrmMain.IsLoaded Then Return
            '顶部条背景
            Dim Brush = New LinearGradientBrush With {.EndPoint = New Point(1, 0), .StartPoint = New Point(0, 0)}
            If ThemeNow = 5 Then
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0, .Color = New MyColor().FromHSL2(ColorHue, ColorSat, 25)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0.5, .Color = New MyColor().FromHSL2(ColorHue, ColorSat, 15)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 1, .Color = New MyColor().FromHSL2(ColorHue, ColorSat, 25)})
                FrmMain.PanTitle.Background = Brush
                FrmMain.PanTitle.Background.Freeze()
            ElseIf ThemeNow <> 12 AndAlso ThemeDontClick <> 2 Then
                Dim Deltas = ColorHueTopbarDelta.Switch(Function(d) New Integer() {-d, 0, d}, Function(d) d)
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0, .Color = New MyColor().FromHSL2(ColorHue + Deltas(0), ColorSat, 48 + ColorLightAdjust)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0.5, .Color = New MyColor().FromHSL2(ColorHue + Deltas(1), ColorSat, 54 + ColorLightAdjust)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 1, .Color = New MyColor().FromHSL2(ColorHue + Deltas(2), ColorSat, 48 + ColorLightAdjust)})
                FrmMain.PanTitle.Background = Brush
                FrmMain.PanTitle.Background.Freeze()
            Else
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0, .Color = New MyColor().FromHSL2(ColorHue - 21, ColorSat, 53 + ColorLightAdjust)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0.33, .Color = New MyColor().FromHSL2(ColorHue - 7, ColorSat, 47 + ColorLightAdjust)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0.67, .Color = New MyColor().FromHSL2(ColorHue + 7, ColorSat, 47 + ColorLightAdjust)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 1, .Color = New MyColor().FromHSL2(ColorHue + 21, ColorSat, 53 + ColorLightAdjust)})
                FrmMain.PanTitle.Background = Brush
            End If
            '标题栏图片
            If ThemeNow >= 5 AndAlso ThemeNow <> 14 Then
                FrmMain.ImgTitle.Source = $"{PathImage}Themes/{ThemeNow}.png"
            Else
                FrmMain.ImgTitle.Source = Nothing
            End If
            FrmMain.ImgTitle.Opacity = If(ThemeNow = 13, 0.25, 0.5)
            '主页面背景
            If Settings.Get(Of Boolean)("UiBackgroundColorful") Then
                Brush = New LinearGradientBrush With {.EndPoint = New Point(0.1, 1), .StartPoint = New Point(0.9, 0)}
                Brush.GradientStops.Add(New GradientStop With {.Offset = -0.1, .Color = New MyColor().FromHSL2(ColorHue - 15, ColorSat * 0.8, 91)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0.4, .Color = New MyColor().FromHSL2(ColorHue, ColorSat * 0.8, 91)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 1.1, .Color = New MyColor().FromHSL2(ColorHue + 15, ColorSat * 0.8, 91)})
                FrmMain.PanForm.Background = Brush
            Else
                FrmMain.PanForm.Background = New MyColor(245, 245, 245)
            End If
            FrmMain.PanForm.Background.Freeze()
        End Sub)
    End Sub
    Friend Sub ThemeCheckAll(EffectSetup As Boolean)
        RunInUi(
        Sub()
            Try
                Dim ThemeId As Integer = Settings.Get(Of Integer)("UiLauncherTheme")
                If Not ThemeCheckOne(ThemeId) Then
                    Logger.Warn($"检测到尚未解锁的主题：{ThemeId}，已重置为默认主题")
                    Settings.Set("UiLauncherTheme", 0)
                End If
                If EffectSetup AndAlso FrmSetupUI IsNot Nothing Then
                    Dim UnlockedTheme As New List(Of Integer)
                    For i = 0 To 14
                        If ThemeCheckOne(i) Then UnlockedTheme.Add(i)
                    Next
                    UnlockedTheme.Add(42)
                    AniControlEnabled += 1
                    If UnlockedTheme.Contains(5) Then
                        FrmSetupUI.RadioLauncherTheme5.Opacity = 1
                        FrmSetupUI.LabLauncherTheme5Unlock.Visibility = Visibility.Collapsed
                        FrmSetupUI.RadioLauncherTheme5Gray.Visibility = Visibility.Collapsed
                    End If
                    If UnlockedTheme.Contains(6) Then FrmSetupUI.RadioLauncherTheme6.IsEnabled = True
                    If UnlockedTheme.Contains(7) Then FrmSetupUI.RadioLauncherTheme7.IsEnabled = True
                    If UnlockedTheme.Contains(8) Then
                        FrmSetupUI.LabLauncherTheme8Copy.Visibility = Visibility.Collapsed
                        FrmSetupUI.RadioLauncherTheme8.IsEnabled = True
                    End If
                    If UnlockedTheme.Contains(9) Then
                        FrmSetupUI.LabLauncherTheme9Copy.Visibility = Visibility.Collapsed
                        FrmSetupUI.RadioLauncherTheme9.IsEnabled = True
                    End If
                    If UnlockedTheme.Contains(10) Then FrmSetupUI.RadioLauncherTheme10.IsEnabled = True
                    If UnlockedTheme.Contains(11) Then
                        FrmSetupUI.LabLauncherTheme11Click.Visibility = Visibility.Collapsed
                        FrmSetupUI.RadioLauncherTheme11.IsEnabled = True
                    ElseIf FrmSetupUI IsNot Nothing AndAlso FrmSetupUI.RadioLauncherTheme11 IsNot Nothing AndAlso FrmSetupUI.RadioLauncherTheme11.IsLoaded Then
                        Dim UnlockedCount As Integer = 0
                        For Each Theme As Integer In UnlockedTheme
                            If Theme >= 5 AndAlso Theme <= 14 Then UnlockedCount += 1
                        Next
                        If UnlockedCount < 3 Then
                            FrmSetupUI.LabLauncherTheme11Click.ToolTip = "需要解锁三个隐藏主题"
                        Else
                            FrmSetupUI.LabLauncherTheme11Click.ToolTip = "点击打开解密游戏入口"
                        End If
                    End If
                    If UnlockedTheme.Contains(12) Then FrmSetupUI.RadioLauncherTheme12.IsEnabled = True
                    If UnlockedTheme.Contains(13) Then FrmSetupUI.RadioLauncherTheme13.IsEnabled = True
                    If UnlockedTheme.Contains(14) Then FrmSetupUI.RadioLauncherTheme14.IsEnabled = True
                    AniControlEnabled -= 1
                End If
            Catch ex As Exception
                If TypeOf If(ex.InnerException, ex) Is FormatException Then
                    Logger.Error(ex, "解锁的主题列表存档已损坏，主题解锁已被重置", LogBehavior.Alert)
                    Settings.Set("UiLauncherThemeHide2", "")
                Else
                    Logger.Error(ex, "检查主题失败", LogBehavior.AlertThenFeedback)
                End If
            End Try
        End Sub)
    End Sub
    Friend Function ThemeCheckOne(Id As Integer) As Boolean
        If {0, 1, 2, 3, 4, 42}.Contains(Id) Then Return True
        If PotatoFeatures.Contains("Theme" & Id) Then Return True
        Select Case Id
            Case 8
                Return CurrentRank >= DonationRank.Rank23
            Case 14
                Return Enumerable.Range(5, 9).Count(Function(i) ThemeCheckOne(i)) >= 5
            Case Else
                If Settings.Get(Of String)("UiLauncherThemeHide").Contains("7") Then ThemeUnlock(7, ShowDoubleHint:=False)
                Return Settings.Get(Of String)("UiLauncherThemeHide2").Split("|", False).Contains(Id.ToString())
        End Select
    End Function
    Friend Function ThemeUnlock(Id As Integer, Optional ShowDoubleHint As Boolean = True, Optional UnlockHint As String = Nothing) As Boolean
        If FrmMain Is Nothing Then Return False
        If Id = 8 Then Throw New Exception("不可使用此方式解锁秋仪金！")
        Dim UnlockedList As New List(Of String)(Settings.Get(Of String)("UiLauncherThemeHide2").Split("|", False))
        If UnlockedList.Contains(Id.ToString) Then
            If ShowDoubleHint Then Hint("该隐藏主题已被解锁！")
            Return False
        End If
        Logger.Info($"解锁隐藏主题：{Id}")
        If UnlockHint IsNot Nothing Then Hint(UnlockHint, HintType.Green, False)
        UnlockedList.Add(Id.ToString)
        Settings.Set("UiLauncherThemeHide2", UnlockedList.Join("|"))
        ThemeCheckAll(True)
        Return True
    End Function

#End Region

#Region "更新"

    Private IsUpdating As Boolean = False
    Friend IsUpdateWaitingRestart As Boolean = False
    Private IsUpdateRestartWaitingTasks As Boolean = False
    '更新标记，用于标识当前构建的更新通道
    Private UpdateMark As String = "EmptyMark********************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************"
    Private Const EcdsaPublicKey As String = "RUNTMSAAAAC4QTUNAewh23Q4Q6koHkyIrDIIZUSbua23sf2DiZmIRwSzadISDRyTVTbuWniH3KR7rKj8XBsabms1be6i3c+S"

    Friend Sub UpdateCheckByButton()
        Static IsUiCheckingUpdate As Boolean
        If IsUiCheckingUpdate Then
            Hint("正在检查更新，请稍候！")
            Return
        End If
        IsUiCheckingUpdate = True
        RunInThread(
        Sub()
            Try
                Hint("正在检查更新，请稍候！")
                ServerLoader.WaitForExit(IsForceRestart:=True)
                Dim IsNewest As Boolean? = PageSetupSystem.IsLauncherNewest()
                If Not IsNewest.HasValue Then
                    Hint("连接 PCL 服务器失败，请确认系统时间是否准确，尝试将 PCL 加入杀毒软件或防火墙白名单，然后重启 PCL！", HintType.Red)
                ElseIf IsNewest.Value Then
                    Hint($"已经是最新{BuildTypeDisplay} {VersionBaseName}，不需要更新啦，可以直接使用！", HintType.Green)
                Else
                    If Not HasUpdatePermission Then
                        InputPotatoCode(IsUpdating:=True)
                        If Not HasUpdatePermission Then Return
                    ElseIf MyMsgBox("发现启动器更新，是否立即下载？", "发现更新！", "更新！冲！", "取消") = 2 Then
                        Return
                    End If
                    UpdateStart(Silent:=False)
                End If
            Catch ex As Exception
                Logger.Error(ex, "尝试手动开始更新失败", LogBehavior.AlertThenFeedback)
            Finally
                IsUiCheckingUpdate = False
            End Try
        End Sub)
    End Sub

    Private Sub UpdateStart(Silent As Boolean, Optional UrlsOverride As IEnumerable(Of String) = Nothing)
        Static IsSilentUpdate As Boolean
        If IsUpdating Then
            If Not Silent Then
                Hint("PCL 正在下载更新，更新结束时将自动重启，请稍候！")
                IsSilentUpdate = False
            End If
            If IsUpdateWaitingRestart Then UpdateRestart(TriggerRestartAndByEnd:=True)
            Return
        End If
        IsUpdating = True
        IsSilentUpdate = Silent
        If Not IsSilentUpdate Then Hint("PCL 正在下载更新，更新结束时将自动重启，请稍候！")
        Try
            Dim Urls As List(Of String) = Nothing
            Dim Extension As String = Nothing
            Dim LocalPath As String = Nothing
            Dim Loader As LoaderCombo(Of String) = Nothing
            Loader = New LoaderCombo(Of String)("启动器更新", {
                New LoaderTask(Of String, List(Of NetFile))("获取更新信息",
                Sub(Task)
                    If ServerLoader.State = LoadState.Loading Then ServerLoader.WaitForExit()
                    If UrlsOverride IsNot Nothing Then
                        Urls = UrlsOverride.ToList()
                    Else
                        If ServerConfig IsNot Nothing AndAlso ServerConfig("Update") IsNot Nothing AndAlso ServerConfig("Update")(BuildType.ToString) IsNot Nothing Then
                            Dim UpdateInfo As JObject = CType(ServerConfig("Update")(BuildType.ToString), JObject)
                            If UpdateInfo.ContainsKey("PatchUrls") AndAlso UpdateMark.StartsWithF("Empty") Then
                                Urls = UpdateInfo("PatchUrls").ToObject(Of List(Of String))()
                            ElseIf UpdateInfo.ContainsKey("DownloadUrls") Then
                                Urls = UpdateInfo("DownloadUrls").ToObject(Of List(Of String))()
                            End If
                        End If
                    End If
                    If Urls Is Nothing OrElse Not Urls.Any Then
                        Logger.Error("更新失败：服务器没有提供有效的更新包下载链接，或无法连接到服务器！", LogBehavior.Alert)
                        IsUpdating = False
                        Return
                    End If
                    Urls = Urls.Select(Function(u) ArgumentReplace(u, AddressOf WebUtility.HtmlEncode)).ToList()
                    Extension = "." & PathUtils.GetExtension(Urls.First.BeforeLast("{CDN}"))
                    LocalPath = $"{RequestTaskTempFolder()}{VersionBaseName}.{CInt(BuildType)}{Extension}"
                    Logger.Info($"更新开始，静默：{IsSilentUpdate}，扩展名：{Extension}，下载目标：{LocalPath}，URL：{Urls.Join("，")}")
                    Task.Output = New List(Of NetFile) From {New NetFile(Urls, LocalPath)}
                End Sub) With {.ProgressWeight = 0.1},
                New LoaderDownload("下载更新文件", New List(Of NetFile)) With {.ProgressWeight = 1},
                New LoaderTask(Of String, Integer)("安装更新",
                Sub(Task)
                    Dim TargetPath As String = Paths.Base & "PCL\Plain Craft Launcher 2.exe"
                    If FileUtils.Exists(TargetPath) Then
                        FileUtils.Delete(TargetPath)
                        Logger.Info($"已清理已存在的更新文件：{TargetPath}")
                    End If
                    If Extension = ".zip" Then
                        Logger.Info($"解压更新文件：{LocalPath}")
                        FileUtils.ExtractToDirectory(LocalPath, Paths.Base & "PCL\")
                    Else
                        Logger.Info($"应用补丁文件：{PathExe} + {LocalPath} → {Paths.Base}PCL\Plain Craft Launcher 2.exe")
                        MsDelta.Apply(PathExe, LocalPath, $"{Paths.Base}PCL\Plain Craft Launcher 2.exe")
                    End If
                    If IsSilentUpdate Then
                        IsUpdateWaitingRestart = True
                        Return
                    End If
                    If McLaunchLoader.State = LoadState.Loading Then
                        Hint("更新已准备就绪，PCL 将在游戏启动完成后重启！", HintType.Green)
                        While McLaunchLoader.State = LoadState.Loading
                            Thread.Sleep(10)
                        End While
                    End If
                    UpdateRestart(TriggerRestartAndByEnd:=True)
                End Sub) With {.ProgressWeight = 0.1}
            })
            Loader.OnStateChanged =
            Sub(StateLoader As LoaderBase)
                If Loader.State = LoadState.Failed Then
                    If IsSilentUpdate Then
                        Logger.Warn(Loader.Error, "启动器静默更新失败")
                    ElseIf Loader.Error.GetDisplay(False).Contains("(403)") Then
                        MyMsgBox("你的系统时间可能并不准确，导致下载验证未通过。" & vbCrLf & "请在校对、修改系统时间后再次尝试更新。", "启动器更新失败")
                    ElseIf Loader.Error.GetDisplay(False).Contains("(404)") Then
                        MyMsgBox($"未找到更新包。可能的原因有：{vbCrLf}- 你所使用的 PCL 不是官方版本{vbCrLf}- 服务器异常", "启动器更新失败")
                    ElseIf TypeOf Loader.Error Is FileNotFoundException Then
                        If MyMsgBox($"由于被 Windows 安全中心拦截，或者存在权限问题，导致 PCL 无法更新。{vbCrLf}请将 PCL 所在文件夹加入白名单！", "更新失败", "查看帮助", "确定", "", IsWarn:=True) = 1 Then
                            CustomEvent.Raise(CustomEvent.EventType.打开帮助, "启动器/Microsoft Defender 添加排除项.json")
                        End If
                    Else
                        Logger.Error(Loader.Error, "启动器更新失败", LogBehavior.AlertThenFeedback)
                    End If
                End If
                If Loader.State = LoadState.Finished OrElse Loader.State = LoadState.Failed OrElse Loader.State = LoadState.Canceled Then
                    IsUpdating = False
                End If
            End Sub
            Loader.Start()
            RunInUi(
            Sub()
                If Not IsSilentUpdate Then
                    LoaderTaskbarAdd(Loader)
                    FrmMain.BtnExtraDownload.ShowRefresh()
                End If
            End Sub)
        Catch ex As Exception
            Logger.Error(ex, "开始启动器更新失败", LogBehavior.AlertThenFeedback)
            IsUpdating = False
        End Try
    End Sub

    Public Sub UpdateRestart(TriggerRestartAndByEnd As Boolean)
        Try
            IsUpdateWaitingRestart = False
            Dim FileName As String = Paths.Base & "PCL\Plain Craft Launcher 2.exe"
            Dim Arguments As String = $"--update {Process.GetCurrentProcess().Id} ""{AppDomain.CurrentDomain.SetupInformation.ApplicationName}"" ""{AppDomain.CurrentDomain.SetupInformation.ApplicationName}"" {TriggerRestartAndByEnd}"
            Logger.Info($"更新程序启动，参数：{Arguments}")
            StartProcess(New ProcessStartInfo(FileName) With {.WindowStyle = ProcessWindowStyle.Hidden, .CreateNoWindow = True, .Arguments = Arguments})
            If TriggerRestartAndByEnd Then
                FrmMain.EndProgram(SendWarning:=False)
                Logger.Info("已由于更新强制结束程序")
            End If
        Catch ex As System.ComponentModel.Win32Exception
            Logger.Warn(ex, "自动更新时触发 Win32 错误，疑似被拦截")
            If MyMsgBox($"由于被 Windows 安全中心拦截，或者存在权限问题，导致 PCL 无法更新。{vbCrLf}请将 PCL 所在文件夹加入白名单，或者手动用 {Paths.Base}PCL\Plain Craft Launcher 2.exe 替换当前文件！", "更新失败", "查看帮助", "确定", "", IsWarn:=True) = 1 Then
                CustomEvent.Raise(CustomEvent.EventType.打开帮助, "启动器/Microsoft Defender 添加排除项.json")
            End If
        End Try
    End Sub

    Public Sub UpdateReplace(ProcessId As Integer, OldFileName As String, NewFileName As String, TriggerRestart As Boolean)
        Try
            Process.GetProcessById(ProcessId).Kill()
        Catch
        End Try
        Dim OldFilePath As String = Path.Combine(PathUtils.AddSlashSuffix(PathUtils.RemoveLastPart(Paths.Base)), PathUtils.GetLastPart(OldFileName))
        Dim NewFilePath As String = Path.Combine(PathUtils.AddSlashSuffix(PathUtils.RemoveLastPart(Paths.Base)), PathUtils.GetLastPart(NewFileName))
        Dim LastException As Exception = Nothing
        Dim RetryCount As Integer = 0
        Do
            Try
                FileUtils.Delete(OldFilePath)
                FileUtils.Delete(NewFilePath)
                If Not FileUtils.Exists(OldFilePath) AndAlso Not FileUtils.Exists(NewFilePath) Then Exit Do
            Catch ex As Exception
                LastException = ex
            End Try
            Thread.Sleep(RetryCount * 500 + 500)
            RetryCount += 1
        Loop While RetryCount <= 4
        Dim GetFailureHint As Func(Of String, String) =
        Function(Reason As String)
            Dim Lines As String() = {
                $"由于{Reason}，PCL 无法完成更新。{vbCrLf}请依次尝试：{vbCrLf}",
                If(PathUtils.IsParentOf(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Paths.Base) OrElse
                   PathUtils.IsParentOf(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Paths.Base),
                   $" - 将 PCL 文件移动到桌面、文档、C 盘以外的文件夹（这或许可以一劳永逸地解决权限问题）{vbCrLf}", ""),
                "",
                "",
                $" - 手动剪切 PCL 文件夹中的新版本程序，覆盖原程序：{vbCrLf}   将位于 {PathExe}{vbCrLf}   的程序{vbCrLf}   剪切到 {NewFilePath} 并覆盖即可。"
            }
            Dim DiskName As Char? = PathUtils.GetDiskName(Paths.Base)
            Lines(2) = If(DiskName.HasValue AndAlso DiskName.Value = "C"c, $" - 将 PCL 文件移动到 C 盘以外的文件夹（这或许可以一劳永逸地解决权限问题）{vbCrLf}", "")
            Lines(3) = If(Not WindowsUtils.HasAdminRole(), $" - 右键 PCL，选择以管理员身份运行{vbCrLf}", "")
            Return String.Concat(Lines)
        End Function
        If FileUtils.Exists(OldFilePath) OrElse FileUtils.Exists(NewFilePath) Then
            If TypeOf LastException Is UnauthorizedAccessException Then
                MsgBox($"{GetFailureHint("删除老版本文件时权限不足")}{vbCrLf}{vbCrLf}详细错误信息：{vbCrLf}{LastException.GetDisplay(True)}", MsgBoxStyle.Critical, "更新失败")
            Else
                MsgBox($"{GetFailureHint("删除老版本文件时出错")}{vbCrLf}{vbCrLf}详细错误信息：{vbCrLf}{LastException.GetDisplay(True)}", MsgBoxStyle.Critical, "更新失败")
            End If
            Return
        End If
        Try
            FileUtils.Copy(PathExe, NewFilePath)
        Catch ex As UnauthorizedAccessException
            MsgBox($"{GetFailureHint("复制新版本文件时权限不足")}{vbCrLf}{vbCrLf}详细错误信息：{vbCrLf}{ex.GetDisplay(True)}", MsgBoxStyle.Critical, "更新失败")
        Catch ex As Exception
            MsgBox($"{GetFailureHint("复制新版本文件时出错")}{vbCrLf}{vbCrLf}详细错误信息：{vbCrLf}{ex.GetDisplay(True)}", MsgBoxStyle.Critical, "更新失败")
            Return
        End Try
        If Not TriggerRestart Then Return
        Try
            StartProcess(NewFilePath)
        Catch ex As Exception
            MsgBox($"PCL 更新已完成，但在自动重启 PCL 时遇到了意外，请手动启动 PCL。{vbCrLf}{vbCrLf}详细错误信息：{vbCrLf}{ex.GetDisplay(True)}", MsgBoxStyle.Information, "更新完成")
        End Try
    End Sub

    ''' <summary>
    ''' 确保 PathTemp/Latest.exe 是最新正式版的 PCL，它会被用于整合包打包。
    ''' 如果不是，则下载一个。
    ''' </summary>
    Friend Sub DownloadLatestPCL(Optional LoaderToSyncProgress As LoaderBase = Nothing)
        Dim CurrentVersion As Integer = If(FileUtils.Exists(PathTemp & "Latest.exe"), Settings.Get(Of Integer)("SystemHighestSavedBetaVersionReg"), 0)
        Dim LatestVersion As Integer = 0
        Dim NoticeText As String = If(FileUtils.TryReadAsString(PathTemp & "Cache\Notice.cfg"), "")
        If NoticeText.Split("|", False).Count >= 3 Then LatestVersion = CInt(Val(NoticeText.Split("|", False)(2)))
        If CurrentVersion >= LatestVersion Then Return
        Logger.Info($"需要下载最新正式版：{CurrentVersion} -> {LatestVersion}")
        If ServerConfig Is Nothing Then
            ServerLoader.WaitForExit(IsForceRestart:=ServerLoader.State = LoadState.Finished)
        End If
        If ServerConfig Is Nothing Then Throw New Exception("无法从服务器获取正式版 PCL 下载链接！")
        NetDownloadByLoader(ServerConfig("Update")("Release")("DownloadUrls").Select(Function(t) t.ToString).ToList,
                            PathTemp & "Latest.zip", LoaderToSyncProgress, New FileChecker With {.MinSize = 1024 * 1024})
        FileUtils.ExtractToDirectory(PathTemp & "Latest.zip", PathTemp)
        FileUtils.Delete(PathTemp & "Latest.zip")
        FileUtils.Move(PathTemp & "Plain Craft Launcher 2.exe", PathTemp & "Latest.exe")
        Settings.Set("SystemHighestSavedBetaVersionReg", LatestVersion)
    End Sub

#End Region

#Region "联网配置"

    ''' <summary>
    ''' 联网获取的配置信息。
    ''' 若获取失败或仍在获取中，可能为 Nothing。
    ''' </summary>
    Public ServerConfig As JObject

    Public ServerLoader As New LoaderTask(Of Integer, Integer)("PCL 配置更新", AddressOf ServerSub, Priority:=ThreadPriority.BelowNormal) With
        {.ReloadTimeout = 1000 * 60 * 60} '超时 1 小时

    Private Sub ServerSub(Loader As LoaderTask(Of Integer, Integer))
        '清理更新缓存
        Try
            If FileUtils.Exists(Paths.Base & "PCL\update.exe") Then
                FileUtils.Delete(Paths.Base & "PCL\update.exe")
                Logger.Info("已清理更新缓存")
            End If
        Catch ex As Exception
            Logger.Warn(ex, "清理更新缓存失败")
        End Try
        '读取本地配置
        Dim LocalVersion As Integer = 0
        Dim ConfigPath As String = PathTemp & "Cache\ServerConfig.json"
        Try
            If FileUtils.Exists(ConfigPath) Then
                ServerConfig = CType(FileUtils.ReadAsJson(ConfigPath), JObject)
                LocalVersion = CInt(Math.Round(Val(ServerConfig("Version"))))
                Logger.Info($"已读取本地缓存的配置：{LocalVersion}")
            End If
        Catch ex As Exception
            Logger.Warn(ex, "读取配置信息失败")
            ServerConfig = Nothing
            FileUtils.Delete(ConfigPath)
        End Try
        Loader.Progress = 0.2
        '获取公告
        Dim LocalNoticeId As Integer = Settings.Get(Of Integer)("HintNotice")
        Dim NoticePath As String = PathTemp & "Cache\Notice.cfg"
        Dim NoticeText As String
        Dim NoticeId As Integer
        Try
            NoticeText = NetRequestByClientRetry("https://pcl2-server-1253424809.file.myqcloud.com/notice.cfg{CDN}")
            NoticeId = CInt(Math.Round(Val(NoticeText.Split("|", False)(0))))
            If NoticeId = 0 Then Throw New Exception("获取到的内容有误！（" & NoticeText & "）")
            If NoticeId > LocalNoticeId Then
                Logger.Info($"本地公告编号：{LocalNoticeId}，服务器公告：{NoticeText}，需更新")
            Else
                Logger.Info($"本地公告编号：{LocalNoticeId}，服务器公告：{NoticeText}，无需更新")
                '公告未更新时，说明当前版本已经是最新正式版，直接将自身复制为 Latest.exe
                If Settings.Get(Of Integer)("SystemHighestSavedBetaVersionReg") < VersionCode OrElse Not FileUtils.Exists(PathTemp & "Latest.exe") Then
                    Settings.Set("SystemHighestSavedBetaVersionReg", VersionCode)
                    Logger.Info($"复制自身为最新正式版：版本号升高到 {VersionCode}")
                    Try
                        FileUtils.Copy(PathExe, PathTemp & "Latest.exe")
                    Catch ex As Exception
                        Logger.Warn(ex, "复制自身为最新正式版失败")
                    End Try
                End If
            End If
            FileUtils.Write(NoticePath, NoticeText)
        Catch ex As Exception
            If TypeOf ex Is InvalidOperationException AndAlso ex.Message.Contains("FIPS") Then
                If MyMsgBox($"由于系统未启用 FIPS 兼容算法，PCL 可能无法正常运行。{vbCrLf}请按照教程启用该功能，然后重启 PCL。", "兼容性警告", "打开教程", "取消", "", IsWarn:=True) = 1 Then
                    OpenWebsite("https://blog.csdn.net/qq_37608398/article/details/81209922")
                End If
            Else
                Logger.Warn(ex, "获取 PCL 服务器状态失败")
                FileUtils.Delete(NoticePath)
            End If
            Return
        End Try
        Loader.Progress = 0.4
        '更新配置
        Try
            Dim ServerVersion As Double = Val(NoticeText.Split("|", False)(4))
            If LocalVersion < ServerVersion Then
                Logger.Info($"本地配置版本号：{LocalVersion}，服务器配置版本号：{ServerVersion}，需要更新")
                NetDownloadByClient("https://pcl2-server-1253424809.file.myqcloud.com/ServerConfig.json{CDN}", ConfigPath)
                ServerConfig = CType(FileUtils.ReadAsJson(ConfigPath), JObject)
            End If
        Catch ex As Exception
            Logger.Warn(ex, "下载配置信息失败")
        End Try
        Loader.Progress = 0.6
        '获取公告详情
        Dim NoticeJson As String
        Try
            If NoticeId <= LocalNoticeId AndAlso FileUtils.Exists(PathTemp & "Cache\Notice.json") Then
                NoticeJson = FileUtils.ReadAsString(PathTemp & "Cache\Notice.json")
            Else
                NoticeJson = NetRequestByClientRetry("https://pcl2-server-1253424809.file.myqcloud.com/notice.json{CDN}", Accept:="*/*", RequireJson:=True)
                FileUtils.Write(PathTemp & "Cache\Notice.json", NoticeJson)
            End If
            Settings.Set("HintNotice", NoticeId)
        Catch ex As Exception
            Logger.Warn(ex, "下载 PCL 服务器公告失败")
            FileUtils.Delete(PathTemp & "Cache\Notice.json")
            Return
        End Try
        Loader.Progress = 0.9
        '处理公告
        Try
            For Each NoticeToken As JToken In CType(NoticeJson.Replace("{UNIQUE}", Identify).DeserializeJson(), JArray)
                Dim Notice As JObject = CType(NoticeToken, JObject)
                Dim NoticeCurrentId As Integer = CInt(Math.Round(Val(Notice("id"))))
                If NoticeCurrentId <= LocalNoticeId Then Continue For
                '检查条件
                Dim IsMatch As Boolean = True
                Dim Requirements = TryCast(Notice("requirements"), JObject)
                If Requirements IsNot Nothing Then
                    For Each Requirement As JProperty In CType(Requirements, IEnumerable(Of JToken))
                        If Not ServerRequirement(Requirement.Name, Requirement.Value) Then IsMatch = False
                    Next
                End If
                If Not IsMatch Then Continue For
                Dim ImportantLevel As Integer = 2
                If Notice("importantLevel") IsNot Nothing Then ImportantLevel = CInt(Notice("importantLevel"))
                Dim IsUpdateNotice As Boolean = False
                If Notice("isUpdate") IsNot Nothing Then IsUpdateNotice = CBool(Notice("isUpdate"))
                Logger.Info($"重要等级 {ImportantLevel}，更新公告 {IsUpdateNotice}")
                If Settings.Get(Of Integer)(If(IsUpdateNotice, "SystemSystemUpdate", "SystemSystemActivity")) > ImportantLevel OrElse (IsUpdateNotice AndAlso IsUpdating) Then Continue For
                '显示公告
                Dim Title As String = If(Notice("title") Is Nothing, "公告", Notice("title").ToString)
                Dim Description As String = If(Notice("description") Is Nothing, "", Notice("description").ToString.Replace("\n", vbCrLf))
                Dim Buttons = TryCast(Notice("buttons"), JArray)
                Dim Actions As JToken
                Dim ButtonCount As Integer = If(Buttons Is Nothing, 0, Buttons.Count)
                If ButtonCount > 0 Then
                    Logger.Info($"显示公告 {NoticeCurrentId}：{Description}")
                    Dim Result As Integer
                    Select Case ButtonCount
                        Case 1
                            Result = MyMsgBox(Description, Title, If(Buttons(0)("text") Is Nothing, "确定", Buttons(0)("text").ToString), "", "", IsWarn:=False, HighLight:=True, ForceWait:=True)
                        Case 2
                            Result = MyMsgBox(Description, Title, If(Buttons(0)("text") Is Nothing, "确定", Buttons(0)("text").ToString), If(Buttons(1)("text") Is Nothing, "确定", Buttons(1)("text").ToString))
                        Case 3
                            Result = MyMsgBox(Description, Title, If(Buttons(0)("text") Is Nothing, "确定", Buttons(0)("text").ToString), If(Buttons(1)("text") Is Nothing, "确定", Buttons(1)("text").ToString), If(Buttons(2)("text") Is Nothing, "确定", Buttons(2)("text").ToString))
                        Case Else
                            Logger.Warn($"公告 {NoticeCurrentId} 的弹窗有 {ButtonCount} 个按钮，无法显示")
                            Continue For
                    End Select
                    Result = MathUtils.Clamp(Result, 1, ButtonCount)
                    Actions = Buttons(Result - 1)("actions")
                Else
                    Actions = Notice("actions")
                End If
                If Actions Is Nothing Then Continue For
                '执行行动
                For Each Action As JProperty In CType(Actions, IEnumerable)
                    Try
                        Select Case Action.Name
                            Case "copy"
                                ClipboardSet(Action.Value.ToString)
                            Case "website"
                                OpenWebsite(Action.Value.ToString)
                            Case "stop"
                                FrmMain.EndProgram(SendWarning:=False)
                            Case "setup"
                                Dim ForbiddenKeys As String() = {"LaunchAdvanceRun", "LaunchAdvanceJvm", "LaunchAdvanceGame", "UiCustomNet",
                                    "HintCustomCommand", "LaunchArgumentJavaAll", "LaunchArgumentJavaSelect", "SystemSystemCache",
                                    "LaunchFolderSelect", "LaunchFolders", "ToolDownloadCert", "CacheAuthServerServer"}
                                For Each Item As KeyValuePair(Of String, JToken) In CType(Action.Value, JObject)
                                    If ForbiddenKeys.Contains(Item.Key) Then Throw New Exception("被禁止的设置项：" & Item.Key)
                                    Settings.Set(Item.Key, Item.Value.ToString)
                                Next
                            Case "update", "slientupdate"
                                Dim IsSilentUpdateAction As Boolean = Action.Name = "slientupdate"
                                If Not HasUpdatePermission Then
                                    If Not IsSilentUpdateAction Then InputPotatoCode(IsUpdating:=True)
                                    If Not HasUpdatePermission Then Continue For
                                End If
                                UpdateStart(IsSilentUpdateAction)
                            Case Else
                                Throw New Exception("未知的行动支：" & Action.Name & ", " & Action.Value.ToString)
                        End Select
                    Catch ex As Exception
                        Logger.Error(ex, $"执行 PCL 服务器公告动作失败（{Action.Name}, {Action.Value}）", LogBehavior.Toast)
                    End Try
                Next
            Next
        Catch ex As Exception
            Logger.Warn(NoticeJson)
            Logger.Error(ex, $"读取 PCL 服务器公告失败（{NoticeJson.Length}）", LogBehavior.Toast)
            FileUtils.Delete(PathTemp & "Cache\Notice.json")
        End Try
    End Sub

    Private Function ServerRequirement(Name As String, Value As JToken) As Boolean
        Try
            Select Case Name
                Case "d10000 <="
                    Return RandomInteger(1, 10000) <= Val(Value)
                Case "opencount <="
                    Return Settings.Get(Of Integer)("SystemCount") <= Val(Value)
                Case "opencount >="
                    Return Settings.Get(Of Integer)("SystemCount") >= Val(Value)
                Case "versioncode <="
                    Return VersionCode <= Val(Value)
                Case "versioncode >="
                    Return VersionCode >= Val(Value)
                Case "versionbranch <="
                    Return CInt(BuildType) <= Val(Value)
                Case "versionbranch >="
                    Return CInt(BuildType) >= Val(Value)
                Case "uniqueaddress ="
                    Return Identify = Value.ToString
                Case "unlockedtheme ="
                    Return ThemeCheckOne(CInt(Math.Round(Val(Value))))
                Case "unlockedtheme !="
                    Return Not ThemeCheckOne(CInt(Math.Round(Val(Value))))
                Case "setupinteger >="
                    Return Settings.Get(Of Integer)(CType(Value, JProperty).Name) >= Val(CType(Value, JProperty).Value)
                Case "setupinteger <="
                    Return Settings.Get(Of Integer)(CType(Value, JProperty).Name) <= Val(CType(Value, JProperty).Value)
                Case "setupboolean ="
                    Return Settings.Get(Of Boolean)(CType(Value, JProperty).Name) = CType(Value, JProperty).Value.ToObject(Of Boolean)()
                Case "debug ="
                    Return ModeDebug.ToString().Lower() = Value.ToString().Lower()
                Case Else
                    Logger.Warn($"未知的条件支：{Name}, {Value}")
                    Return False
            End Select
        Catch ex As Exception
            Logger.Warn($"判断分支条件失败：{Name}, {Value}")
            Return False
        End Try
    End Function

#End Region

#Region "赞助等级"

    Public ReadOnly Property CurrentRank As DonationRank
        Get
            For Each Rank As DonationRank In {DonationRank.Rank98, DonationRank.Rank54, DonationRank.Rank23, DonationRank.Rank12, DonationRank.Rank6}
                If PotatoFeatures.Contains(Rank.ToString) Then Return Rank
            Next
            Return DonationRank.None
        End Get
    End Property

    Private ReadOnly Property HasUpdatePermission As Boolean
        Get
            If CurrentRank < DonationRank.Rank12 Then Return PotatoFeatures.Contains("Update")
            Return True
        End Get
    End Property

    Private _PotatoFeatures As List(Of String) = Nothing

    ''' <summary>
    ''' 当前已解锁的土豆特性列表。
    ''' </summary>
    Private ReadOnly Property PotatoFeatures As List(Of String)
        Get
            If _PotatoFeatures Is Nothing Then
                _PotatoFeatures = New List(Of String)
                For Each Key As String In Settings.Get(Of String)("Potatoes").Split("|"c, True)
                    Dim Result = CheckPotatoCode(Key)
                    If Result.Is(Of List(Of String))() Then _PotatoFeatures.AddRange(Result.As(Of List(Of String))())
                Next
                _PotatoFeatures = _PotatoFeatures.Distinct().ToList()
            End If
            Return _PotatoFeatures
        End Get
    End Property

    Public Sub InputPotatoCode(IsUpdating As Boolean)
        Dim Input As String
        If IsUpdating Then
            If MyMsgBox($"请点击【复制命令】，然后将复制的命令粘贴到爱发电私信，获取土豆码。{vbCrLf}输入土豆码后，就能继续更新啦！", "验证爱发电赞助", "输入土豆码", "复制命令", "取消",
                        IsWarn:=False, HighLight:=True, ForceWait:=False,
                        Button2Action:=Sub()
                                          ClipboardSet("土豆 " & Identify, False)
                                          OpenWebsite("https://meloong.com/afd/message/df2dfad2960911e89c5252540025c377?potato")
                                      End Sub) = 3 Then Return
            Input = MyMsgBoxInput("输入土豆码", $"请在【爱发电】向龙腾猫跃发送【土豆 {Identify}】，然后输入自动回复的土豆码以继续更新。{vbCrLf}自动回复可能比较慢，发送后等一会儿刷新一下网页应该就有了。", Nothing, New ObjectModel.Collection(Of Validate) From {New ValidateNullOrWhiteSpace()})
        Else
            Input = MyMsgBoxInput("输入土豆码", $"请输入你获得的土豆码。{vbCrLf}你的识别码：{Identify}", Nothing, New ObjectModel.Collection(Of Validate) From {New ValidateNullOrWhiteSpace()})
        End If
        If String.IsNullOrWhiteSpace(Input) Then Return
        Input = Input.ReplaceLineEndings("").Between("：", "在").Trim(" "c, "【"c, "】"c)
        If String.IsNullOrWhiteSpace(Input) Then Return
        Logger.Info($"输入的土豆码：{Input}")
        If Not IsUpdating AndAlso (Input = "PuzzleUnlock" & Identify.Substring(Identify.Count - 4) OrElse
                                   Input = "PuzzleUnlock" & Identify.Substring(Identify.Count - 5) OrElse
                                   Input = "PuzzleUnlock" & Identify.Substring(Identify.Count - 6)) Then
            MyMsgBox($"隐藏主题 极客蓝 已解锁！{vbCrLf}难道，这就是终点了？还是说……？", "解锁新隐藏主题！")
            ThemeUnlock(11)
            Return
        End If
        Try
            Dim Result = CheckPotatoCode(Input)
            If Result.Is(Of String)() Then Throw New Exception(Result.As(Of String)())
            Dim Features = Result.As(Of List(Of String))()
            Logger.Info($"土豆码包含的特性：{Features.Join(", ")}")
            If Features.Any(Function(f) Not PotatoFeatures.Contains(f)) Then
                Dim PotatoList = Settings.Get(Of String)("Potatoes").Split("|"c, True).ToList()
                PotatoList.Add(Input)
                Settings.Set("Potatoes", PotatoList.Join("|"))
                _PotatoFeatures = Nothing
            End If
            If IsUpdating Then
                If Not HasUpdatePermission Then Throw New Exception("这个土豆码里不包含更新权限……")
            Else
                Hint("已成功解锁！", HintType.Green)
            End If
        Catch ex As Exception
            Logger.Info($"土豆码无效：{ex.Message}")
            If MyMsgBox(ex.Message, "错误", "重新输入", "取消", "", IsWarn:=True) = 1 Then InputPotatoCode(IsUpdating)
        End Try
    End Sub
    Friend Sub GeneratePotatoCode()
    End Sub

    ''' <summary>
    ''' 校验土豆码。
    ''' 若校验成功，返回其包含的特性列表；否则返回错误提示。
    ''' </summary>
    Private Function CheckPotatoCode(Key As String) As OneOf(Of List(Of String), String)
        If String.IsNullOrEmpty(Key) Then
            Return "输入的土豆码为空。"
        ElseIf Key.Length < 30 AndAlso Key.ContainsIgnoreCase(Identify) Then
            Return $"你输入的是识别码，不是土豆码。{vbCrLf}要获取土豆码，请在爱发电私信发送【土豆 {Identify}】。"
        ElseIf Key.Length > 85 AndAlso Key.Length < 90 AndAlso Key.EndsWithF("=") Then
            Return $"你输入的是解锁码，不是土豆码。{vbCrLf}要获取土豆码，请在爱发电私信发送【土豆】。"
        ElseIf Key.Length > 150 AndAlso Key.Length < 165 AndAlso Key.EndsWithF("11") Then
            Return $"你输入的是更新密钥，不是土豆码。{vbCrLf}要获取土豆码，请在爱发电私信发送【土豆】。"
        ElseIf Not Key.StartsWithF("CD", True) OrElse Not Key.Contains("#") Then
            Return $"你输入的不是土豆码……有一说一，这是啥玩意儿？{vbCrLf}要获取土豆码，请在爱发电私信发送【土豆 {Identify}】。"
        ElseIf Key(2) > "1"c Then
            Return $"这个土豆码得在新版 PCL 上才能使用。{vbCrLf}请更新 PCL，或是在爱发电私信发送【下载】，然后在新下载的 PCL 上输入土豆码。"
        ElseIf "1"c > Key(3) Then
            Return $"这个土豆码不太新鲜，已经过期了。{vbCrLf}请在爱发电私信重新发送【土豆 {Identify}】，获取更新鲜的土豆码。"
        ElseIf Not Key.StartsWithF("CD") Then
            Return $"土豆码区分大小写，但你把一些大写的东西输成小写的了。{vbCrLf}你可以选中文本，然后右键复制粘贴，不必手动输入。"
        End If
        Try
            Dim Parts As String() = Key.Substring(4).Replace("p", "=").Replace("%", "p").Split("#"c)
            Dim Content As String = Encoding.UTF8.GetString(Convert.FromBase64String(Parts(1)))
            CryptographyUtils.EcdsaVerify(Identify & Key(2) & Key(3) & Content, Parts(0), EcdsaPublicKey)
            Return Content.Split(","c, True).ToList()
        Catch ex As Exception
            Logger.Warn(ex, $"土豆码校验失败（{Key}）")
            Return $"土豆码有误，请检查：{vbCrLf}- 私信显示的识别码应当为 {Identify}，如果不是，请在私信发送【土豆 {Identify}】{vbCrLf}- 你输入的以下土豆码是否与私信中的一致：{vbCrLf}{Key}{vbCrLf}{vbCrLf}错误原因：{ex.Message}"
        End Try
    End Function

    ''' <summary>
    ''' 获取设备识别码。
    ''' </summary>
    Friend Function GetIdentify() As String
        Try
            If ApplicationStartTick < 1 Then Return "0000-0000-0000-0000"
            Dim BoardId As String
            Try
                BoardId = RegistryUtils.TryRead("HKEY_LOCAL_MACHINE\SYSTEM\HardwareConfig", "LastConfig", "Unknown").ToString().Upper().Trim("{"c, "}"c)
            Catch ex As Exception
                Logger.Warn(ex, "获取主板标识码失败")
                BoardId = "Unknown"
            End Try
            Dim IdentifySeed As String
            Try
                IdentifySeed = Settings.Get(Of String)("Identify")
            Catch
                IdentifySeed = ""
            End Try
            If IdentifySeed.Length < 3 Then
                IdentifySeed = GetTimeMs().ToString() & My.Computer.Info.AvailablePhysicalMemory.ToString()
                Settings.Set("Identify", IdentifySeed)
            End If
            Dim Hash As String = (BoardId & IdentifySeed).GetStableHashCode().ToString("X").EnsureLength("7"c, 16)
            Return $"{Hash.Substring(4, 4)}-{Hash.Substring(12, 4)}-{Hash.Substring(0, 4)}-{Hash.Substring(8, 4)}"
        Catch ex As Exception
            Logger.Error(ex, $"PCL 无法获取设备识别码，这可能会导致部分设置无法正常存储。{vbCrLf}{vbCrLf}详细的错误信息", LogBehavior.AlertThenFeedback)
            Return "0000-0000-0000-0000"
        End Try
    End Function

#End Region

End Module
