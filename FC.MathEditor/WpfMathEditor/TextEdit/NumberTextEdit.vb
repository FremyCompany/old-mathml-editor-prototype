﻿Public Class NumberTextEdit : Inherits TextEdit

    Public Overrides Function Clone_Internal() As MathElement
        Dim Clone As New OperatorTextEdit()
        For Each C In Children
            Clone.Children.Add(C.Clone())
        Next : Return Clone
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            ' TODO: Check MathML
            Return "mn"
        End Get
    End Property

End Class