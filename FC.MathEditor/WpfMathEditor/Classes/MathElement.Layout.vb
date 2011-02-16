Partial Public Class MathElement

    Public Event SizeChanged As EventHandler
    Public Event VisualChanged As EventHandler

    ' Palatino Linotype, 
    Public FM1 As New FontFamily("Candara")
    Public FM2 As New FontFamily("Cambria Math")
    Public Overridable ReadOnly Property DefaultFontFamily As FontFamily
        Get
            Return FM1
        End Get
    End Property

    Public Overridable ReadOnly Property DefaultMathFontFamily As FontFamily
        Get
            Return FM2
        End Get
    End Property


    Public Overridable ReadOnly Property DefaultFontStyle As FontStyle
        Get
            Return FontStyles.Normal
        End Get
    End Property

    Public Overridable ReadOnly Property DefaultFontWeight As FontWeight
        Get
            Return FontWeights.Normal
        End Get
    End Property

    Public Overridable ReadOnly Property DefaultFontStretch As FontStretch
        Get
            Return FontStretches.Normal
        End Get
    End Property

    Private _Font As Typeface
    Public Property Font As Typeface
        Get
            If _Font Is Nothing Then _Font = New Typeface(FontFamily, FontStyle, FontWeight, FontStretch)
            Return _Font
        End Get
        Set(ByVal value As Typeface)

        End Set
    End Property

    Protected _FontFamily As FontFamily, _IsFontFamilyDefined As Boolean = False
    Public Property FontFamily As FontFamily
        Get
            If _FontFamily Is Nothing Then
                If Parent Is Nothing Then
                    Return DefaultFontFamily
                Else
                    _FontFamily = Parent.FontFamily
                    Return _FontFamily
                End If
            Else
                Return _FontFamily
            End If
        End Get
        Set(ByVal value As FontFamily)
            SetFontFamily(value, True)
        End Set
    End Property

    Private Sub SetFontFamily(ByVal value As FontFamily, ByVal DefineProperty As Boolean)
        If (DefineProperty) OrElse (Not _IsFontFamilyDefined) Then
            _FontFamily = value
            _IsFontFamilyDefined = DefineProperty AndAlso (value IsNot Nothing)
            For Each C In Me.Children
                C.SetFontFamily(value, False)
            Next
        End If
    End Sub

    Protected _FontStretch As FontStretch?, _IsFontStretchDefined As Boolean = False
    Public Property FontStretch As FontStretch?
        Get
            If _FontStretch Is Nothing Then
                If Parent Is Nothing Then
                    Return DefaultFontStretch
                Else
                    _FontStretch = Parent.FontStretch
                    Return _FontStretch
                End If
            Else
                Return _FontStretch
            End If
        End Get
        Set(ByVal value As FontStretch?)
            SetFontStretch(value, True)
        End Set
    End Property

    Private Sub SetFontStretch(ByVal value As FontStretch?, ByVal DefineProperty As Boolean)
        If (DefineProperty) OrElse (Not _IsFontStretchDefined) Then
            _FontStretch = value
            _IsFontStretchDefined = DefineProperty AndAlso (value IsNot Nothing)
            For Each C In Me.Children
                C.SetFontStretch(value, False)
            Next
        End If
    End Sub

    Protected _FontStyle As FontStyle?, _IsFontStyleDefined As Boolean = False
    Public Property FontStyle As FontStyle?
        Get
            If _FontStyle Is Nothing Then
                If Parent Is Nothing Then
                    Return DefaultFontStyle
                Else
                    _FontStyle = Parent.FontStyle
                    Return _FontStyle
                End If
            Else
                Return _FontStyle
            End If
        End Get
        Set(ByVal value As FontStyle?)
            SetFontStyle(value, True)
        End Set
    End Property

    Private Sub SetFontStyle(ByVal value As FontStyle?, ByVal DefineProperty As Boolean)
        If (DefineProperty) OrElse (Not _IsFontStyleDefined) Then
            _FontStyle = value
            _IsFontStyleDefined = DefineProperty AndAlso (value IsNot Nothing)
            For Each C In Me.Children
                C.SetFontStyle(value, False)
            Next
        End If
    End Sub

    Protected _FontWeight As FontWeight?, _IsFontWeightDefined As Boolean = False
    Public Property FontWeight As FontWeight?
        Get
            If _FontWeight Is Nothing Then
                If Parent Is Nothing Then
                    Return DefaultFontWeight
                Else
                    _FontWeight = Parent.FontWeight
                    Return _FontWeight
                End If
            Else
                Return _FontWeight
            End If
        End Get
        Set(ByVal value As FontWeight?)
            SetFontWeight(value, True)
        End Set
    End Property

    Private Sub SetFontWeight(ByVal value As FontWeight?, ByVal DefineProperty As Boolean)
        If (DefineProperty) OrElse (Not _IsFontWeightDefined) Then
            _FontWeight = value
            _IsFontWeightDefined = DefineProperty AndAlso (value IsNot Nothing)
            For Each C In Me.Children
                C.SetFontWeight(value, False)
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
