Partial Public MustInherit Class MathElement

    REM 
    REM XML Hierarchy
    REM

    Private _Parent As MathElement
    Public Property Parent As MathElement
        Get
            Return _Parent
        End Get
        Set(ByVal value As MathElement)
            If _Parent IsNot Nothing Then
                Throw New InvalidOperationException("Unable to modify the parent of an element after it has been set. Use Clone() to get a parent-free copy of this element.")
            Else
                If value IsNot Nothing Then
                    _Parent = value
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property Root As MathElement
        Get
            If _ParentDocument IsNot Nothing Then Return _ParentDocument
            If _Parent Is Nothing Then Return Me
            Return Parent.Root
        End Get
    End Property

    Private _ParentDocument As MathDocument
    Public ReadOnly Property ParentDocument As MathDocument
        Get

            ' Cache the parent document, if not already done
            If _ParentDocument Is Nothing Then
                If Parent IsNot Nothing Then
                    _ParentDocument = Parent.ParentDocument
                End If
            End If

            ' Return the cached parent document
            Return _ParentDocument

        End Get
    End Property

    Public ReadOnly Property NextSibling As MathElement
        Get
            If _Parent Is Nothing Then Return Nothing
            Return _Parent.Children.After(Me)
        End Get
    End Property

    Public ReadOnly Property PreviousSibling As MathElement
        Get
            If _Parent Is Nothing Then Return Nothing
            Return _Parent.Children.Before(Me)
        End Get
    End Property

    Public Function IsBefore(ByVal Elm As MathElement) As Boolean

        If Elm Is Nothing Then Return True
        If Elm.Parent IsNot Parent Then Return False

        Do 'PreviousSiblig
            If (Elm Is Me) Then Return True
            Elm = Parent.Children.Before(Elm)
        Loop While (Elm IsNot Nothing)

        Return False
    End Function

    Public Function IsAfter(ByVal Elm As MathElement) As Boolean

        If Elm Is Nothing Then Return True
        Return Elm.IsBefore(Me)

    End Function

    Public Function Clone() As MathElement
        Clone = Me.Clone_Internal()
        If _IsFontStretchDefined Then Clone.FontStretch = Me.FontStretch
        If _IsFontWeightDefined Then Clone.FontWeight = Me.FontWeight
        If _IsFontStyleDefined Then Clone.FontStyle = Me.FontStyle
        If _IsFontFamilyDefined Then Clone.FontFamily = Me.FontFamily
        If _IsFontSizeDefined Then Clone.FontSize = Me.FontSize
        If _IsForegroundDefined Then Clone.Foreground = Me.Foreground
        If _IsBackgroundDefined Then Clone.Background = Me.Background
        Return Clone
    End Function
    Public MustOverride Function Clone_Internal() As MathElement

    REM
    REM XML Children
    REM

    Protected _Children As ChildrenHelper
    Public ReadOnly Property Children As ChildrenHelper
        Get
            If _Children Is Nothing Then _Children = Nothing
            Return _Children
        End Get
    End Property

    Public Sub AddChild(ByVal NewChild As MathElement)
        Children.Add(NewChild)
    End Sub

    Public Sub RemoveChild(ByVal OldChild As MathElement)
        Children.Remove(OldChild)
    End Sub

    Public Sub ReplaceChild(ByVal OldChild As MathElement, ByVal NewChild As MathElement)
        Children.Replace(OldChild, NewChild)
    End Sub

    Public ReadOnly Property FirstChild As MathElement
        Get
            Return Children.First
        End Get
    End Property

    Public ReadOnly Property LastChild As MathElement
        Get
            Return Children.Last
        End Get
    End Property

    Public ReadOnly Property TreeDepht As Integer
        Get
            TreeDepht = 0 : Dim P As MathElement = Me
            While P.Parent IsNot Nothing
                TreeDepht += 1 : P = P.Parent
            End While
            Return TreeDepht
        End Get
    End Property

    Public Function GetCommonAncestrorWith(ByVal el1 As MathElement) As MathElement
        Return GetCommonAncestrorBetween(el1, Me)
    End Function

    Public Shared Function GetCommonAncestrorBetween(ByVal el1 As MathElement, ByVal el2 As MathElement)

        Dim Delta As Integer = el2.TreeDepht - el1.TreeDepht
        If Delta > 0 Then
            For x = 1 To Delta
                el2 = el2.Parent
            Next
        ElseIf Delta < 0 Then
            For x = 1 To -Delta
                el1 = el1.Parent
            Next
        End If

        While el2 IsNot el1
            el1 = el1.Parent : el2 = el2.Parent
        End While

        Return el1

    End Function

    ''' <summary>
    ''' Risen when a property of the current element changed, or a property of one of its children
    ''' </summary>
    ''' <remarks></remarks>
    Public Event Changed As EventHandler
    Private Sub RaiseChanged()
        RaiseEvent Changed(Me, EventArgs.Empty)
    End Sub

    Private Sub MathElement_Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Changed
        If _Parent IsNot Nothing Then Parent.RaiseChanged()
    End Sub

    ' TODO: RemovedFromParent
    Public Event RemovedFromParent As EventHandler

    Private Sub MathElement_RemovedFromParent(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.RemovedFromParent
        _Parent = Nothing
        _ParentDocument = Nothing
        _Selection = Nothing
    End Sub
End Class
