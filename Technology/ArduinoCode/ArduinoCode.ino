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
    
    door = true;
    stopTrain();
    delay(1000);
    door = false;
    stopTrain();
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
    Serial.println(currentStation);
    delay(150);
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
      Serial.println("Arduino Received: " + message);
      delay(100);
      startStation = message.toInt();
      message = "";
    }
    else if (received == '\n')
    {
      Serial.println("Arduino Received: " + message);
      delay(100);
      endStation = message.toInt();
      hasReceived = true;
      message = "";
    }
    else 
    {
      message += received;
    }
  }
}
