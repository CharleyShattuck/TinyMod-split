\ main.fs
target

\ Arduino constants
0 wconstant INPUT
1 wconstant OUTPUT
2 wconstant INPUT_PULLUP

\ serial mode's array 
variable data 4 ramALLOT \ 6 bytes in all
: /data  data a! 5 #, for false c!+ next ; 

: initPortExpanders
    $20 #, initMCP23017
    $21 #, initMCP23017 ;
: initPins
    INPUT_PULLUP  6 #, pinMode
    INPUT_PULLUP  8 #, pinMode
    OUTPUT 4 #, pinMode  1 #, 4 #, !pin ;
: init  initPortExpanders initPins ;

\ read the keyboard
: @pins (  - n)
    $20 #, @MCP23017  $21 #, @MCP23017
    16 #, lshift or ;

\ get one stroke
: press (  - n)  false begin drop @pins until ;
: release ( n1 - n2)  begin @pins while or repeat drop ;
: scan (  - n)
    begin press 30 #, ms @pins if or release exit then drop drop again

\ vectored emit, for testing
wvariable 'spit \ execution tokens are 16 bit
: spit  'spit w@ execute ; 
: >emit  ['] emit 'spit w! ; 
: >hc.  ['] hc. 'spit w! ; 
: send  data a! 5 #, for c@+ spit next ;
: ?send  data a! \ TX Bolt
    c@+ if dup spit then drop
    c@+ if dup $40 #, or spit then drop
    c@+ if dup $80 #, or spit then drop
    c@+ if $c0 #, or spit exit then spit ;

\ Gemini protocol to the data array
: mark ( mask a)  data + dup >r c@ or r> c! ; 
: Gemini ( n)  /data $80 #, data c!
    dup     $2000 #, and if $40 #, 1 #, mark then drop \ S1
    dup     $4000 #, and if $20 #, 1 #, mark then drop \ S2
    dup     $1000 #, and if $10 #, 1 #, mark then drop \ T
    dup      $800 #, and if $08 #, 1 #, mark then drop \ K
    dup      $200 #, and if $04 #, 1 #, mark then drop \ P
    dup      $400 #, and if $02 #, 1 #, mark then drop \ W
    dup       $02 #, and if $01 #, 1 #, mark then drop \ H
    dup       $04 #, and if $40 #, 2 #, mark then drop \ R
    dup       $20 #, and if $20 #, 2 #, mark then drop \ A
    dup       $40 #, and if $10 #, 2 #, mark then drop \ O 
    dup       $08 #, and if $08 #, 2 #, mark then drop \ *
    dup       $10 #, and if $04 #, 2 #, mark then drop \ *
    dup $80000000 #, and if $04 #, 2 #, mark then drop \ *
    dup    $10000 #, and if $04 #, 2 #, mark then drop \ *
    dup   $400000 #, and if $08 #, 3 #, mark then drop \ E
    dup   $800000 #, and if $04 #, 3 #, mark then drop \ U
    dup $20000000 #, and if $02 #, 3 #, mark then drop \ F
    dup $40000000 #, and if $01 #, 3 #, mark then drop \ R
    dup $10000000 #, and if $40 #, 4 #, mark then drop \ P
    dup  $4000000 #, and if $20 #, 4 #, mark then drop \ B
    dup  $8000000 #, and if $10 #, 4 #, mark then drop \ L
    dup  $2000000 #, and if $08 #, 4 #, mark then drop \ G
    dup    $20000 #, and if $04 #, 4 #, mark then drop \ T
    dup    $40000 #, and if $02 #, 4 #, mark then drop \ S
    dup    $80000 #, and if $01 #, 4 #, mark then drop \ D
    dup   $100000 #, and if $01 #, 5 #, mark then drop \ Z
    dup       $80 #, and if $40 #, 5 #, mark then drop \ #
    dup   $200000 #, and if $40 #, 5 #, mark then drop \ #
    drop ; 
: send-Gemini  Gemini send ;

\ TX Bolt
: send-TXBolt  abort ;

\ A-Z
: send-A-Z  abort ;

\ NKRO keyboard mode
cvariable former
: spew ( c - )
    dup Keyboard.press
    former c@ if dup Keyboard.release then
    drop former c! ; 
: send-NKRO ( n - )
    false former c!
    dup     $2000 #, and if/ [ char q ] #, spew then
    dup     $1000 #, and if/ [ char w ] #, spew then
    dup      $200 #, and if/ [ char e ] #, spew then
    dup       $02 #, and if/ [ char r ] #, spew then
    dup       $08 #, and if/ [ char t ] #, spew then \
    dup $80000000 #, and if/ [ char y ] #, spew then
    dup $20000000 #, and if/ [ char u ] #, spew then
    dup $10000000 #, and if/ [ char i ] #, spew then
    dup  $8000000 #, and if/ [ char o ] #, spew then
    dup    $20000 #, and if/ [ char p ] #, spew then
    dup    $80000 #, and if/ [ char [ ] #, spew then \
    dup     $4000 #, and if/ [ char a ] #, spew then
    dup      $800 #, and if/ [ char s ] #, spew then
    dup      $400 #, and if/ [ char d ] #, spew then
    dup       $04 #, and if/ [ char f ] #, spew then
    dup       $10 #, and if/ [ char g ] #, spew then \
    dup    $10000 #, and if/ [ char h ] #, spew then
    dup $40000000 #, and if/ [ char j ] #, spew then
    dup  $4000000 #, and if/ [ char k ] #, spew then
    dup  $2000000 #, and if/ [ char l ] #, spew then
    dup    $40000 #, and if/ [ char ; ] #, spew then
    dup   $100000 #, and if/ [ char ' ] #, spew then \
    dup       $20 #, and if/ [ char c ] #, spew then
    dup       $40 #, and if/ [ char v ] #, spew then
    dup       $80 #, and if/ [ char 3 ] #, spew then
    dup   $200000 #, and if/ [ char 4 ] #, spew then
    dup   $400000 #, and if/ [ char n ] #, spew then
    dup   $800000 #, and if/ [ char m ] #, spew then \
    drop Keyboard.releaseAll ; 

\ slider switch determines protocol
: @sliders ( - n)
    8 #, @pin invert 2 #, and
    6 #, @pin invert 1 #, and or ;

create protocols
    ', send-Gemini
    ', send-NKRO
    ', send-TXBolt
    ', abort

turnkey decimal init Keyboard.begin >hc.
    begin @sliders 3 #, = while/ interpret repeat >emit
    begin scan @sliders protocols + @p execute
    again
\    >hc. interpret
\    >emit go

