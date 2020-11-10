#include <LiquidCrystal_I2C.h>
#include <Servo.h>

#define servoPin 8
#define pirPin 9

Servo myservo;
int pos = 0;

bool door = false;

int stopCount = 0;
int stopAmount = 2;

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
    lcd.clear();
    lcd.setCursor(2, 0);
    lcd.print("Arrived"); 
    stopCount = 0;
  }
  else{  
    for (pos = 90; pos >= 0; pos --) {
    myservo.write(pos);  
    delay(5);                
    }
    lcd.clear();
      lcd.setCursor(2, 0);
      lcd.print("Left");
  }   
}

void checkMovement(){
  //bool pirStatus = digitalRead(pirPin);
  bool buttonStatus = digitalRead(2);

  if(buttonStatus){
    stopCount++;
    Serial.println(stopCount);
    delay(150);
  }

  if(stopCount == stopAmount){
    door = true;
    stopTrain();
    delay(1000);
    door = false;
    stopTrain();
  }
}

void loop() {
  checkMovement();
  
  /*if (Serial.available() > 0)
  {
    char received = Serial.read();

    if (received == '\n')
    {
      Serial.println("Arduino Received: " + message);
      delay(100);
      StopAmount = message.toInt();
      StopCount = 0;
      door = false;
      stopTrain();
      Serial.println("Train has left the station");
      lcd.clear();
      lcd.setCursor(2, 0);
      lcd.print("Left");
    }
    else 
    {
      message += received;
    }
  }*/
}
