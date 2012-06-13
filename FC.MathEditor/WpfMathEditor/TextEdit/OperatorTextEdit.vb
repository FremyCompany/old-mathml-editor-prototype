Partial Public Class OperatorTextEdit : Inherits TextEdit

    Public Sub New()
        ' Do nothing
    End Sub

    Public Sub New(Children As IEnumerable(Of MathElement))
        Call Me.New()
        For Each C In Children
            Me.Children.Add(C)
        Next
    End Sub

    Public ReadOnly Property IsOperator As Boolean
        Get

            ' Empty MO are operators
            Return Not (IsFence OrElse IsSeparator OrElse IsAccent)

        End Get
    End Property

    Public ReadOnly Property IsFence As Boolean
        Get

            Try

                Dim Result = False : Dim Str As String = Nothing

                If TryGetAttribute("fence", Str) Then
                    Return Parsers.ForBoolean.Parse(Str, Me)
                End If

                If OperatorDic_fence.TryGetValue(Me.ToString(), Result) Then
                    Return Result
                Else
                    Return False
                End If

            Catch

                Return False

            End Try

        End Get
    End Property

    Public ReadOnly Property IsSeparator As Boolean
        Get

            Try

                Dim Result = False : Dim Str As String = Nothing

                If TryGetAttribute("separator", Str) Then
                    Return Parsers.ForBoolean.Parse(Str, Me)
                End If

                If OperatorDic_separator.TryGetValue(Me.ToString(), Result) Then
                    Return Result
                Else
                    Return False
                End If

            Catch

                Return False

            End Try

        End Get
    End Property

    Public ReadOnly Property IsAccent As Boolean
        Get

            Try

                Dim Result = False : Dim Str As String = Nothing

                If TryGetAttribute("accent", Str) Then
                    Return Parsers.ForBoolean.Parse(Str, Me)
                End If

                If OperatorDic_accent.TryGetValue(Me.ToString(), Result) Then
                    Return Result
                Else
                    Return False
                End If

            Catch

                Return False

            End Try

        End Get
    End Property

    Public ReadOnly Property IsStretchy As Boolean
        Get

            Try

                Dim Result = False : Dim Str As String = Nothing

                If TryGetAttribute("stretchy", Str) Then
                    Return Parsers.ForBoolean.Parse(Str, Me)
                End If

                If OperatorDic_stretchy.TryGetValue(Me.ToString(), Result) Then
                    Return Result
                Else
                    Return False
                End If

            Catch

                Return False

            End Try

        End Get
    End Property

    Public ReadOnly Property IsSymmetric As Boolean
        Get

            Try

                Dim Result = True : Dim Str As String = Nothing

                If TryGetAttribute("symmetric", Str) Then
                    Return Parsers.ForBoolean.Parse(Str, Me)
                End If

                If OperatorDic_symmetric.TryGetValue(Me.ToString(), Result) Then
                    Return Result
                Else
                    Return True
                End If

            Catch

                Return True

            End Try

        End Get
    End Property

    Public Overrides Function TryGetDefaultAttribute(AttributeName As String, ByRef Result As String) As Boolean
        Select Case AttributeName
            Case "mathvariant"
                Result = "serif" : Return True

            Case "separator"
                Dim bResult, xResult As Boolean
                xResult = OperatorDic_separator.TryGetValue(Me.ToString(), bResult)
                If xResult Then
                    Result = bResult : Return True
                Else
                    Return False
                End If

            Case "fence"
                Dim bResult, xResult As Boolean
                xResult = OperatorDic_fence.TryGetValue(Me.ToString(), bResult)
                If xResult Then
                    Result = bResult : Return True
                Else
                    Return False
                End If

            Case "accent"
                Dim bResult, xResult As Boolean
                xResult = OperatorDic_accent.TryGetValue(Me.ToString(), bResult)
                If xResult Then
                    Result = bResult : Return True
                Else
                    Return False
                End If

            Case Else
                Return MyBase.TryGetDefaultAttribute(AttributeName, Result)

        End Select
    End Function

    Protected Overrides Function Clone_Internal(ByRef ShouldCloneChildren As Boolean) As MathElement
        Return New OperatorTextEdit()
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            Return "mo"
        End Get
    End Property

    Public Overrides Function IsCharAccepted(C As Integer, Position As Integer) As Boolean
        Return (Position = 0)
    End Function

    Public Overrides ReadOnly Property EatInputByDefault As Boolean
        Get
            Return False
        End Get
    End Property

End Class
