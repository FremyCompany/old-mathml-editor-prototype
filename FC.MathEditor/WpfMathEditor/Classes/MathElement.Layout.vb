Partial Public Class MathElement

    Public Event SizeChanged As EventHandler
    Public Event VisualChanged As EventHandler

    ' Palatino Linotype, 
    Public Shared DefaultFont As New Typeface(New FontFamily("Cambria Math"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal)

    Protected _Font As Typeface, _IsFontDefined As Boolean = False
    Public Property Font As Typeface
        Get
            If _Font Is Nothing Then
                If Parent Is Nothing Then
                    Return DefaultFont
                Else
                    _Font = Parent.Font
                    Return _Font
                End If
            Else
                Return _Font
            End If
        End Get
        Set(ByVal value As Typeface)
            SetFont(value, True)
        End Set
    End Property

    Private Sub SetFont(ByVal value As Typeface, ByVal DefineProperty As Boolean)
        If (DefineProperty) OrElse (Not _IsFontDefined) Then
            _Font = value
            _IsFontDefined = DefineProperty AndAlso (value IsNot Nothing)
            For Each C In Me.Children
                C.SetFont(value, False)
            Next
        End If
    End Sub

    Protected _Foreground As Color?, _IsForegroundDefined As Boolean
    Public Property Foreground As Color?
        Get
            If _Foreground Is Nothing Then
                If Parent Is Nothing Then
                    Return SystemColors.WindowTextColor
                Else
                    _Foreground = Parent.Foreground
                    Return _Foreground
                End If
            Else
                Return _Foreground
            End If
        End Get
        Set(ByVal value As Color?)
            SetForeground(value, True)
        End Set
    End Property

    Private Sub SetForeground(ByVal value As Color?, ByVal DefineProperty As Boolean)
        If (DefineProperty) OrElse (Not _IsForegroundDefined) Then
            _Foreground = value
            _IsForegroundDefined = DefineProperty AndAlso (value IsNot Nothing)
            For Each C In Me.Children
                C.SetForeground(value, False)
            Next
        End If
    End Sub

    Protected _FontSize As Double?, _IsFontSizeDefined As Boolean
    Public Property FontSize As Double?
        Get
            If _FontSize Is Nothing Then
                If Parent Is Nothing Then
                    Return 20
                Else
                    _FontSize = Parent.FontSize
                    Return _FontSize
                End If
            Else
                Return _FontSize
            End If
        End Get
        Set(ByVal value As Double?)
            SetFontSize(value, True)
        End Set
    End Property

    Private Sub SetFontSize(ByVal value As Double?, ByVal DefineProperty As Boolean)
        If (DefineProperty) OrElse (Not _IsFontSizeDefined) Then
            _FontSize = value
            _IsFontSizeDefined = DefineProperty AndAlso (value IsNot Nothing)
            For Each C In Me.Children
                C.SetFontSize(value, False)
            Next
        End If
    End Sub

    Protected _Background As Color?, _IsBackgroundDefined As Boolean
    Public Property Background As Color?
        Get
            If _Background Is Nothing Then
                If Parent Is Nothing Then
                    Return Colors.Transparent
                Else
                    _Background = Parent.Background
                    Return _Background
                End If
            Else
                Return _Background
            End If
        End Get
        Set(ByVal value As Color?)
            SetBackground(value, True)
        End Set
    End Property

    Private Sub SetBackground(ByVal value As Color?, ByVal DefineProperty As Boolean)
        If (DefineProperty) OrElse (Not _IsBackgroundDefined) Then
            _Background = value
            _IsBackgroundDefined = DefineProperty AndAlso (value IsNot Nothing)
            For Each C In Me.Children
                C.SetBackground(value, False)
            Next
        End If
    End Sub

End Class
