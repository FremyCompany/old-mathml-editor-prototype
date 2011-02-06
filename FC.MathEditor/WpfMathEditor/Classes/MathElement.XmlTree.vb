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

    Public MustOverride Function Clone() As MathElement

    REM
    REM XML Children
    REM

    Private _Children As ChildrenHelper
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

End Class
