    mov     $1, 255
    mov     $0, 1
loop:
    cmp     $0, 1
    je      sobe
    dec     $2
    cmp     $2, 0
    je      changeup
    jump    loop
sobe:
    inc     $2
    cmp     $2, $1
    je      changedown
    jump    loop
changeup:
    mov     $0, 1
    jump    loop
changedown:
    mov     $0, 0
    jump    loop