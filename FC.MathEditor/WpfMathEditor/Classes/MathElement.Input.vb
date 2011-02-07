Partial Public Class MathElement

    Private _Input As InputHelper
    Public Property Input As InputHelper
        Get
            If Me.ParentDocument Is Nothing Then Return Nothing
            Return _Input
        End Get
        Set(ByVal value As InputHelper)
            _Input = value
        End Set
    End Property

    Private _Selection As SelectionHelper
    Public Overridable ReadOnly Property Selection As SelectionHelper
        Get
            If Me.ParentDocument Is Nothing Then Return Nothing
            If _Selection Is Nothing Then _Selection = ParentDocument.Selection
            Return _Selection
        End Get
    End Property

    Public Event GotFocus As EventHandler
    Public Event LostFocus As EventHandler

    Public Overridable Function GetNextInputElement(ByVal CurrentElement As MathElement) As MathElement
        If CurrentElement Is Nothing Then Return Children.First
        Return Children.After(CurrentElement)
    End Function

    Public Overridable Function GetPreviousInputElement(ByVal CurrentElement As MathElement) As MathElement
        If CurrentElement Is Nothing Then Return Children.Last
        Return Children.Before(CurrentElement)
    End Function

    Public ReadOnly Property ParentLayoutEngine As MathElement
        Get
            Dim P As MathElement = Parent
            If Parent Is Nothing Then
                Throw New InvalidOperationException("Unable to retreive a ParentLayoutEngine from an unparented element.")
            End If

            While P IsNot Nothing
                If P.Children.IsLayoutEngine Then
                    Return P
                Else
                    P = P.Parent
                End If
            End While

            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ParentLayoutEngineChild As MathElement
        Get
            Dim P As MathElement = Me
            If Parent Is Nothing Then
                Throw New InvalidOperationException("Unable to retreive a ParentLayoutEngine from an unparented element.")
            End If

            While P IsNot Nothing
                If P.Parent.Children.IsLayoutEngine Then
                    Return P
                Else
                    P = P.Parent
                End If
            End While

            Return Nothing
        End Get
    End Property


End Class
