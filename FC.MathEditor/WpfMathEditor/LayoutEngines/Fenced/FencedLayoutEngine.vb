Partial Public Class FencedLayoutEngine : Inherits MathElement

    Public Sub New(Optional Open As String = Nothing, Optional Close As String = Nothing, Optional Separators As String = Nothing, Optional Children As IEnumerable(Of MathElement) = Nothing)

        MyBase.New()

        Me.CloseAtt = Close
        Me.OpenAtt = Open
        Me.SepAtt = Separators

        If Children IsNot Nothing Then
            For Each El In Children
                Me.Children.Add(El)
            Next
        End If

    End Sub

    '++
    '++ Attributes
    '++

    Private OpenAtt As String
    Private CloseAtt As String
    Private SepAtt As String

    Public Property OpenAttribute() As String
        Get
            Return OpenAtt
        End Get
        Set(value As String)
            OpenAtt = value : RaiseChanged()
        End Set
    End Property

    Public Property CloseAttribute() As String
        Get
            Return CloseAtt
        End Get
        Set(value As String)
            CloseAtt = value : RaiseChanged()
        End Set
    End Property

    Public Property SeparatorsAttribute() As String
        Get
            Return SepAtt
        End Get
        Set(value As String)
            SepAtt = value : RaiseChanged()
        End Set
    End Property

    '++
    '++ Methods
    '++

    Public Overrides Function Clone_Internal(Optional CloneChildren As Boolean = True) As MathElement
        Return New FencedLayoutEngine(OpenAttribute, CloseAttribute, SeparatorsAttribute)
    End Function

    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper
        Return New LayoutEngineChildrenHelper(Me)
    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper
        Return New FencedLayoutEngineExportHelper(Me)
    End Function

    Protected Overrides Function GetInitialInputHelper() As InputHelper
        Return New FencedLayoutEngineInputHelper(Me)
    End Function

End Class
