Public Class TextEditInputHelper : Inherits InputHelper

    Private IsAccepted As Func(Of Integer, Boolean) = Function(C) DirectCast(This, TextEdit).IsAccepted(C)
    Public Sub New(This As TextEdit, IsAccepted As Func(Of Integer, Boolean))
        MyBase.New(This)
    End Sub

    Public Overrides Function ProcessChar_Internal(InputChar As Integer) As Boolean
        '
        ' TODO: TextEdit.ProcessChar_Internal
        '
        ' A TextEdit treat char typed in between by simply accepting it 
        ' or by splitting itself in two parts and by inserting the unaccepted
        ' char in a new text-edit between the two

        If IsAccepted(InputChar) AndAlso This.Children.CanAdd Then

            Dim NewElement = New UnicodeGlyph(InputChar)

            Console.WriteLine("TODO: TextEdit.ProcessChar_Internal")
            This.Selection.ParentElement.Children.InsertAfter(NewElement, This.Selection.PreviousSibling)
            This.Selection.SetSelection(This, NewElement, This.Selection.NextSibling)

            Return True

        Else

            Dim FirstPart As TextEdit = This.Clone(False)
            Dim SecondPart As TextEdit = This.Clone(False)

            For Each child In This.Children
                If child.ChildIndex < This.Selection.GetSelection(SelectionHelper.SelectionPointType.Selection).ChildIndex Then
                    FirstPart.AddChild(child.Clone())
                Else
                    SecondPart.AddChild(child.Clone())
                End If
            Next

            Dim TextEdit As TextEdit = TextEdit.FromChar(InputChar)

            This.ParentElement.Children.InsertAfter(SecondPart, This)
            This.ParentElement.Children.InsertAfter(TextEdit, This)
            This.ParentElement.Children.InsertAfter(FirstPart, This)
            This.RemoveFromParent()

            TextEdit.Selection.SetSelection(TextEdit.ParentElement, TextEdit, SecondPart)

            Return True

        End If
    End Function

    Public Overrides Function ProcessChar_FromLeft_Internal(InputChar As Integer) As Boolean

        If IsAccepted(InputChar) AndAlso This.Children.CanAdd Then

            This.Selection.DeleteContents()

            Dim C = New UnicodeGlyph(InputChar)
            This.Children.InsertAfter(C, Nothing)
            This.Selection.SetSelection(This, C, C.NextSibling)
            Return True

        Else

            Return False

        End If

    End Function

    Public Overrides Function ProcessChar_FromRight_Internal(InputChar As Integer) As Boolean

        If IsAccepted(InputChar) AndAlso This.Children.CanAdd Then

            This.Selection.DeleteContents()

            Dim C = New UnicodeGlyph(InputChar)
            This.AddChild(C)
            This.Selection.SetSelection(This, C, C.NextSibling)
            Return True

        Else

            Return False

        End If

    End Function

    Public Overrides Function ProcessBackSpace_FromRight_Internal() As Boolean
        Return MyBase.ProcessBackSpace_FromRight_Internal()
    End Function

    Public Overrides Function ProcessDelete_FromLeft_Internal() As Boolean
        Return MyBase.ProcessDelete_FromLeft_Internal()
    End Function

End Class
