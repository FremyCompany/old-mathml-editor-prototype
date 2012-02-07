Public Class RootFormatter : Inherits MathElement

    Private Cnt As RowLayoutEngine
    ''' <summary>
    ''' This RLE contains all elements inside the root
    ''' </summary>

    Public ReadOnly Property Content() As RowLayoutEngine
        Get
            Return Cnt
        End Get
    End Property

    Private Exp As RowLayoutEngine
    ''' <summary>
    ''' This RLE contains all elements inside the base
    ''' </summary>
    Public ReadOnly Property Exponent() As RowLayoutEngine
        Get
            Return Exp
        End Get
    End Property

    ''' <summary>
    ''' Returns a value indicating if the element is a simple square root or not.
    ''' </summary>
    Public ReadOnly Property HasExponent As Boolean
        Get
            Return Exp IsNot Nothing
        End Get
    End Property

    <UserCallable()>
    Public Sub ConvertToSquareRoot()


        If HasExponent Then

            Exp = Nothing : _Children = New FormatterChildrenHelper(Me)

        End If

    End Sub

    Public Sub New(Content As RowLayoutEngine, Optional Exponent As RowLayoutEngine = Nothing)

        ' Save fields
        Me.Cnt = Content : Me.Exp = Exponent

        ' Add to children
        Children.Add(Cnt) : If Exp IsNot Nothing Then Children.Add(Exp)
        DirectCast(Children, FormatterChildrenHelper).Freeze()


    End Sub

    Protected Overrides Function Clone_Internal(ByRef ShouldCloneChildren As Boolean) As MathElement

        ' Check the various configurations
        If ShouldCloneChildren Then : ShouldCloneChildren = False
            If Exp Is Nothing Then
                Return New RootFormatter(Cnt.Clone(), Nothing)
            Else
                Return New RootFormatter(Cnt.Clone(), Exp.Clone())
            End If
        Else
            Return New RootFormatter(Cnt.Clone(), Exp.Clone())
        End If

    End Function

    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper

        Return New FormatterChildrenHelper(Me)

    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper

        Return New RootFormatterExportHelper(Me)

    End Function

    Protected Overrides Function GetInitialInputHelper() As InputHelper

        ' TODO: Input on root is a todo
        Return New EmptyInputHelper(Me)

    End Function

End Class
