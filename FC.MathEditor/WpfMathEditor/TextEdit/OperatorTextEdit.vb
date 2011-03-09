﻿Public Class OperatorTextEdit : Inherits TextEdit

    Public Sub New()
        Me.Input = New TextEditInputHelper(Me, Function(C) Not Char.IsLetter(Char.ConvertFromUtf32(C)))
    End Sub

    Public Overrides Function Clone_Internal() As MathElement
        Dim Clone As New OperatorTextEdit()
        For Each C In Children
            Clone.Children.Add(C.Clone())
        Next : Return Clone
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            Return "mo"
        End Get
    End Property

End Class
