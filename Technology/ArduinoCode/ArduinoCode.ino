#include <LiquidCrystal_I2C.h>
#include <Servo.h>

#define servoPin 8
#define pirPin 9

Servo myservo;
int pos = 0;
bool door = false;

int currentStation = 1;
int startStation = 2;
int endStation = 1;

bool atStart;
bool atEnd;
bool hasReceived;
bool startAtEnd;

LiquidCrystal_I2C lcd(0x27, 16, 2);

String message;

void setup() {
  Serial.begin(9600);
  myservo.attach(servoPin);
  pinMode(pirPin, INPUT);
  lcd.begin();
  lcd.backlight();

  pinMode(2, INPUT);
}

void showScreen() {
  if(currentStation == startStation || currentStation == endStation){
    lcd.clear();
    lcd.setCursor(1, 0);
    lcd.print("Train Location:");
    lcd.setCursor(1,1);
    lcd.print(currentStation);
  }
  if(currentStation != endStation || startAtEnd){
    delay(1000);
    lcd.clear();
    if(currentStation == 3){ 
      lcd.setCursor(1, 0);
      lcd.print("Train Location:");
      lcd.setCursor(1,1);
      char info[64];
      sprintf(info, "%d -> 1", currentStation);
      lcd.print(info);
    }
    else{
      lcd.setCursor(1, 0);
      lcd.print("Train Location:");
      lcd.setCursor(1,1);
      char info[64];
      sprintf(info, "%d -> %d", currentStation, currentStation + 1);
      lcd.print(info);
    }
  }
}

void stopTrain(){
  if(door){  
    for (pos = 0; pos <= 90; pos ++) { 
    myservo.write(pos);   
    delay(5);             
    }
    Serial.println("Train has arrived"); 
  }
  else{  
    for (pos = 90; pos >= 0; pos --) {
    myservo.write(pos);  
    delay(5);                
    }
    Serial.println("Train has left the station"); 
  }   
}

void moveTrain(){
  if(currentStation == startStation  && !atStart){
    atStart = true;
    atEnd = false;

    if(startStation != endStation){
      door = true;
      stopTrain();
      delay(1000);
      door = false;
      stopTrain();
    }
  }

  if(atStart && !atEnd){
    if(currentStation == endStation){
      atEnd = true;
      atStart = false;
      hasReceived = false;
      
      door = true;
      stopTrain();
    }
  }
}

void checkMovement(){
  //bool pirStatus = digitalRead(pirPin);
  bool buttonStatus = digitalRead(2);

  if(buttonStatus){
    currentStation++;
    if(currentStation == 4){
      currentStation = 1;
    }
    startAtEnd = false;
    Serial.println(currentStation);
    delay(150);
    showScreen();
  }
}

void loop() {
  if(hasReceived){
    checkMovement(); 
    moveTrain();
  }
  
  if (Serial.available() > 0)
  {
    char received = Serial.read();
    
    if(received == ','){
      char info[64];
      sprintf(info, "CurrentStation: %d", currentStation);
      Serial.println(info);
      delay(150);
      Serial.println("StartStation: " + message);
      delay(100);
      startStation = message.toInt();
      message = "";
    }
    else if (received == '\n')
    {
      Serial.println("EndStation: " + message);
      delay(100);
      endStation = message.toInt();
      if(endStation == currentStation && endStation != startStation){
        startAtEnd = true;
      }
      else{
        startAtEnd = false;
      }
      hasReceived = true;
      message = "";
      showScreen();
    }
    else 
    {
      message += received;
    }
  }
}
