Module QualityControl

#If DEBUG Then

    Public Sub AssertFalse(X As Boolean)
        If X Then Throw New InvalidOperationException("An invalid operation has led to a corrupted state.")
    End Sub

    Public Sub AssertTrue(X As Boolean)
        If Not X Then Throw New InvalidOperationException("An invalid operation has led to a corrupted state.")
    End Sub

    Public Sub AssertNull(X As Object)
        If X IsNot Nothing Then Throw New InvalidOperationException("An invalid operation has led to a corrupted state.")
    End Sub

    Public Sub AssertNotNull(X As Object)
        If X Is Nothing Then Throw New InvalidOperationException("An invalid operation has led to a corrupted state.")
    End Sub

    Public Sub AssertEquals(X As Object, Y As Object)
        If X Is Y Then Throw New InvalidOperationException("An invalid operation has led to a corrupted state.")
    End Sub

#End If

End Module
